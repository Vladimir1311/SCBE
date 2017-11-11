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

namespace SituationCenterCore.Models.Rooms
{
    public class RoomsManager : IRoomManager
    {
        private ILogger<RoomsManager> logger;
        private ApplicationDbContext dataBase;
        private readonly IRoomSecurityManager roomSecyrityManager;

        public RoomsManager(
            ILogger<RoomsManager> logger,
            ApplicationDbContext dataBase,
            IRoomSecurityManager roomSecyrityManager)

        {
            this.logger = logger;
            this.dataBase = dataBase;
            this.roomSecyrityManager = roomSecyrityManager;
        }

        public Room CreateNewRoom(Guid createrId, CreateRoomRequest createRoomInfo)
        {
            if (createRoomInfo.Name.Length > 32)
                throw new StatusCodeException(StatusCode.TooLongRoomName);
            var creater = dataBase.Users
                .Include(U => U.Room)
                .FirstOrDefault(U => U.Id == createrId.ToString())
                ?? throw new Exception("Не существует запрашиваемого пользователя");

            CheckCreatingRoomParams(createRoomInfo, creater);

            Room newRoom = new Room()
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
                    var userIds = dataBase
                        .Users
                        .Where(U => phoneNumbers.Contains(U.PhoneNumber))
                        .Select(U => U.Id)
                        .Select(I => Guid.Parse(I))
                        .ToArray();
                    roomSecyrityManager.CreateInvationRule(newRoom, userIds);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            dataBase.Add(newRoom);
            roomSecyrityManager.AddAdminRole(creater, newRoom);
            creater.RoomId = newRoom.Id;
            dataBase.SaveChanges();
            return newRoom;
        }

        public IEnumerable<Room> FindRooms(Predicate<Room> func)
        {
            return dataBase.Rooms
                .Include(R => R.SecurityRule)
                .Include(R => R.Users)
                .Where(R => func(R))
                .ToList();
        }

        public void JoinToRoom(Guid userId, Guid roomId, string securityData)
        {
            var user = dataBase.Users
                .Include(U => U.Room)
                .FirstOrDefault(U => U.Id == userId.ToString())
                ?? throw new ArgumentException($"not user with id {userId}");

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
            var user = dataBase.Users.FirstOrDefault(U => U.Id == userId.ToString());
            user.RoomId = null;
            dataBase.SaveChanges();
        }

        public void DeleteRoom(Guid userId, Guid roomId)
        {
            var room = FindRoom(roomId);
            var user = room.Users.FirstOrDefault(U => U.Id == userId.ToString());
            if (user == null)
                user = dataBase.Users.FirstOrDefault(U => U.Id == userId.ToString());

            if (user == null)
                throw new Exception("Нет пользователя с указанным Id");
            if (!roomSecyrityManager.CanDelete(user, room))
                throw new Exception("Нет права на удаление комнаты!");

            foreach (var person in room.Users)
                person.RoomId = null;
            //TODO Проанализировать EF, слишком много запросов SaveChanges
            dataBase.SaveChanges();
            roomSecyrityManager.ClearRoles(room);
            dataBase.Rooms.Remove(room);
            dataBase.SaveChanges();
            dataBase.Rules.Remove(room.SecurityRule);
            dataBase.SaveChanges();
        }

        public IEnumerable<Room> Rooms(Guid userId)
        {
            var user = dataBase.Users.FirstOrDefault(U => U.Id == userId.ToString())
                ?? throw new ArgumentException();
            var allrooms = dataBase
                .Rooms
                .Include(R => R.Users)
                .Include(R => R.SecurityRule)
                .ToList();
            return allrooms
                .Where(R => roomSecyrityManager.CanJoin(user, R))
                .ToArray();
        }

        private ApplicationUser FindUser(Guid userId)
        {
            return dataBase.Users.FirstOrDefault(U => U.Id == userId.ToString());
        }

        private void CheckCreatingRoomParams(CreateRoomRequest createRoomInfo, ApplicationUser creater)
        {
            var errorcodes = new List<StatusCode>();

            //TODO Искать только в видимых для пользователя комнатах
            if (dataBase.Rooms.Any(R => R.Name == createRoomInfo.Name))
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
            var rule = await dataBase.Rules
                                     .FirstOrDefaultAsync(R => R.PrivacyRule == PrivacyRoomType.InvationPrivate
                                                     && R.Id == dataBase.Rooms.FirstOrDefault(Room => Room.Id == currentRoomId).Id);
            if (rule.PrivacyRule != PrivacyRoomType.InvationPrivate)
                throw new StatusCodeException(StatusCode.IncrorrecrTargetRoomType);
            rule.Data = string.Join('\n', rule.Data
                .Split('\n')
                .Union(dataBase
                       .Users
                       .Where(U => phones.Contains(U.PhoneNumber))
                       .Select(U => U.Id)));
            await dataBase.SaveChangesAsync();
        }
    }
}