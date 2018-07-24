using Common.Requests.Room.CreateRoom;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SituationCenter.Shared.Exceptions;
using SituationCenterCore.Data;
using SituationCenterCore.Models.Rooms.Security;
using SituationCenterCore.Models.Rooms.VoiceChatModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SituationCenter.Shared.Models.Rooms;
using SituationCenterCore.Services.Interfaces;

namespace SituationCenterCore.Models.Rooms
{
    public class RoomsManager : IRoomManager
    {
        private ILogger<RoomsManager> logger;
        private ApplicationDbContext dataBase;
        private readonly IRoomSecurityManager roomSecyrityManager;
        private readonly IFileServerNotifier fileServerNotifier;

        public RoomsManager(
            ILogger<RoomsManager> logger,
            ApplicationDbContext dataBase,
            IRoomSecurityManager roomSecyrityManager,
            IFileServerNotifier fileServerNotifier)

        {
            this.logger = logger;
            this.dataBase = dataBase;
            this.roomSecyrityManager = roomSecyrityManager;
            this.fileServerNotifier = fileServerNotifier;
        }

        public async Task<Room> CreateNewRoom(Guid createrId, CreateRoomRequest createRoomInfo)
        {
            if (createRoomInfo.Name.Length > 32)
                throw new StatusCodeException(StatusCode.TooLongRoomName);
            var creater = await dataBase.Users
                .Include(U => U.Room)
                .FirstOrDefaultAsync(U => U.Id == createrId)
                ?? throw new Exception("Не существует запрашиваемого пользователя");

            await CheckCreatingRoomParams(createRoomInfo, creater);

            var newRoom = new Room()
            {
                Name = createRoomInfo.Name,
                PeopleCountLimit = createRoomInfo.UsersCountMax
            };
            switch (createRoomInfo.PrivacyType)
            {
                case PrivacyRoomType.Public:
                    roomSecyrityManager.CreatePublicRule(newRoom);
                    break;

                case PrivacyRoomType.Password:
                    var password = createRoomInfo.Password;
                    roomSecyrityManager.CreatePasswordRule(newRoom, password);
                    break;

                case PrivacyRoomType.InvationPrivate:
                    var phoneNumbers = createRoomInfo.Phones
                        .Append(creater.PhoneNumber)
                        .Distinct()
                        .ToArray();
                    var userIds = await dataBase
                        .Users
                        .Where(U => phoneNumbers.Contains(U.PhoneNumber))
                        .Select(U => U.Id)
                        .ToArrayAsync();
                    roomSecyrityManager.CreateInvationRule(newRoom, userIds);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(createRoomInfo.PrivacyType));
            }
            dataBase.Add(newRoom);
            await roomSecyrityManager.AddAdminRole(creater, newRoom);
            creater.RoomId = newRoom.Id;
            await dataBase.SaveChangesAsync();
            await fileServerNotifier.SetRoom(createrId, newRoom.Id);
            return newRoom;
        }


        public void JoinToRoom(Guid userId, Guid roomId, string securityData)
        {
            var user = dataBase.Users
                .Include(U => U.Room)
                .FirstOrDefault(U => U.Id == userId)
                ?? throw new ApiArgumentException($"not user with id {userId}");

            if (user.RoomId != null)
                throw new StatusCodeException(StatusCode.PersonInRoomAtAWrongTime);

            var calledRoom = dataBase
                .Rooms
                .Include(R => R.Users)
                .FirstOrDefault(R => R.Id == roomId)
                ?? throw new StatusCodeException(StatusCode.DontExistRoom);

            if (calledRoom.Users.Count == calledRoom.PeopleCountLimit)
                throw new StatusCodeException(StatusCode.RoomFilled);

            logger.LogDebug("Validate user");
            roomSecyrityManager.Validate(user, calledRoom, securityData);
            logger.LogDebug("Validated user");
            user.RoomId = calledRoom.Id;
            dataBase.SaveChanges();
            //TODO Do async
            fileServerNotifier.SetRoom(userId, roomId).Wait();
        }

        public Room FindRoom(Guid roomId)
        {
            var room = dataBase.Rooms
                .Include(R => R.Users)
                .Include(R => R.SecurityRule)
                .FirstOrDefault(R => R.Id == roomId);
            return room;
        }

        public void LeaveFromRoom(Guid userId)
        {
            var user = dataBase.Users.FirstOrDefault(U => U.Id == userId);
            user.RoomId = null;
            dataBase.SaveChanges();
            fileServerNotifier.SetRoom(userId, null).Wait();
        }

        public void DeleteRoom(Guid userId, Guid roomId)
        {
            var room = FindRoom(roomId) ?? throw new StatusCodeException(StatusCode.DontExistRoom);
            var user = room.Users.FirstOrDefault(U => U.Id == userId);
            if (user == null)
                user = dataBase.Users.FirstOrDefault(U => U.Id == userId);

            if (user == null)
                throw new Exception("Нет пользователя с указанным Id");
            if (!roomSecyrityManager.CanDelete(user, room))
                throw new Exception("Нет права на удаление комнаты!");

            foreach (var person in room.Users)
            {
                person.RoomId = null;
                fileServerNotifier.SetRoom(person.Id, null).Wait();
            }

            //TODO Проанализировать EF, слишком много запросов SaveChanges
            dataBase.SaveChanges();
            roomSecyrityManager.ClearRoles(room);
            dataBase.Rooms.Remove(room);
            dataBase.SaveChanges();
            dataBase.Rules.Remove(room.SecurityRule);
            dataBase.SaveChanges();
        }

        public IQueryable<Room> Rooms(Guid userId)
        {
            var user = dataBase.Users.FirstOrDefault(U => U.Id == userId)
                ?? throw new ApiArgumentException();
            return dataBase
                .Rooms
                .Include(R => R.Users)
                .Include(R => R.SecurityRule)
                .Where(R => roomSecyrityManager.CanJoin(user, R));
        }

        private ApplicationUser FindUser(Guid userId)
        {
            return dataBase.Users.FirstOrDefault(U => U.Id == userId);
        }

        private async Task CheckCreatingRoomParams(CreateRoomRequest createRoomInfo, ApplicationUser creater)
        {
            var errorcodes = new List<StatusCode>();

            //TODO Искать только в видимых для пользователя комнатах
            if (await dataBase.Rooms.AnyAsync(R => R.Name == createRoomInfo.Name))
                errorcodes.Add(StatusCode.RoomNameBusy);

            if (creater.RoomId != null)
                errorcodes.Add(StatusCode.PersonInRoomAtAWrongTime);

            try { createRoomInfo.UsersCountMax = PeopleCount(createRoomInfo.UsersCountMax); }
            catch (StatusCodeException ex) { errorcodes.Add(ex.StatusCode); }
            catch { throw; }

            switch (errorcodes.Count)
            {
                case 0: return;
                case 1: throw new StatusCodeException(errorcodes[0]);
                default:
                    throw new MultiStatusCodeException(errorcodes);
            }
        }

        private int PeopleCount(int peopleCount)
        {
            if (peopleCount <= 0) return 8;
            if (peopleCount > 8 || peopleCount == 1)
                throw new StatusCodeException(StatusCode.MaxPeopleCountInRoomIncorrect);
            return peopleCount;
        }

        public async Task InviteUsersByPhoneToRoom(Guid currentRoomId, List<string> phones)
        {
            var rule = (await dataBase.Rooms.Include(r => r.SecurityRule).FirstOrDefaultAsync(Room => Room.Id == currentRoomId))?.SecurityRule;
            if (rule?.PrivacyRule != PrivacyRoomType.InvationPrivate)
                throw new StatusCodeException(StatusCode.IncrorrecrTargetRoomType);
            rule.Data = string.Join('\n', rule.Data
                .Split('\n')
                .Select(Guid.Parse)
                .Union(dataBase
                       .Users
                       .Where(U => phones.Contains(U.PhoneNumber))
                       .Select(U => U.Id)));
            await dataBase.SaveChangesAsync();
        }
    }
}