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
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SituationCenter.Shared.Models.Rooms;
using SituationCenterCore.Services.Interfaces;

namespace SituationCenterCore.Models.Rooms
{
    public class RoomsManager : IRoomManager
    {
        private readonly ILogger<RoomsManager> logger;
        private readonly ApplicationDbContext dataBase;
        private readonly IRoomSecurityManager roomSecyrityManager;
        private readonly IFileServerNotifier fileServerNotifier;
        private readonly IMapper mapper;

        public RoomsManager(
            ILogger<RoomsManager> logger,
            ApplicationDbContext dataBase,
            IRoomSecurityManager roomSecyrityManager,
            IFileServerNotifier fileServerNotifier,
            IMapper mapper)

        {
            this.logger = logger;
            this.dataBase = dataBase;
            this.roomSecyrityManager = roomSecyrityManager;
            this.fileServerNotifier = fileServerNotifier;
            this.mapper = mapper;
        }

        public async Task<Room> CreateNewRoom(Guid createrId, CreateRoomRequest createRoomInfo)
        {
            if (createRoomInfo.Name.Length > 32)
                throw new StatusCodeException(StatusCode.TooLongRoomName);
            var creater = await dataBase.Users
                .Include(u => u.Room)
                .FirstOrDefaultAsync(u => u.Id == createrId)
                ?? throw new Exception("Не существует запрашиваемого пользователя");

            await CheckCreatingRoomParams(createRoomInfo, creater);

            var newRoom = mapper.Map<Room>(createRoomInfo);

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
                    var users = await dataBase
                        .Users
                        .Where(u => createRoomInfo.InviteUsers.Contains(u.Id))
                        .ToListAsync();
                    roomSecyrityManager.CreateInvationRule(newRoom, users);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(createRoomInfo.PrivacyType));
            }
            roomSecyrityManager.AddAdminRole(creater, newRoom);
            creater.Room = newRoom;
            dataBase.Add(newRoom);
            await dataBase.SaveChangesAsync();
            await fileServerNotifier.SetRoom(createrId, newRoom.Id);
            return newRoom;
        }


        public async Task JoinToRoom(Guid userId, Guid roomId, string securityData)
        {
            var user = await dataBase.Users
                .Include(U => U.Room)
                .FirstOrDefaultAsync(U => U.Id == userId)
                ?? throw new ApiArgumentException($"not user with id {userId}");

            if (user.RoomId != null)
                throw new StatusCodeException(StatusCode.PersonInRoomAtAWrongTime);

            var calledRoom = await dataBase
                .Rooms
                .Include(R => R.Users)
                .FirstOrDefaultAsync(R => R.Id == roomId)
                ?? throw new StatusCodeException(StatusCode.DontExistRoom);

            if (calledRoom.Users.Count == calledRoom.PeopleCountLimit)
                throw new StatusCodeException(StatusCode.RoomFilled);

            logger.LogDebug("Validate user");
            roomSecyrityManager.Validate(user, calledRoom, securityData);
            logger.LogDebug("Validated user");
            user.RoomId = calledRoom.Id;
            await dataBase.SaveChangesAsync();

            await fileServerNotifier.SetRoom(userId, roomId);
        }

        public Task<Room> FindRoom(Guid roomId)
            => dataBase.Rooms
                .Include(r => r.Users)
                    .ThenInclude(u => u.UserRoomRoles)
                .Include(r => r.SecurityRule)
                .SingleOrDefaultAsync(r => r.Id == roomId);


        public async Task LeaveFromRoom(Guid userId)
        {
            var user = await dataBase.Users.FirstOrDefaultAsync(u => u.Id == userId);
            user.RoomId = null;
            await dataBase.SaveChangesAsync();
            await fileServerNotifier.SetRoom(userId, null);
        }
        public async Task DeleteRoom(Guid userId, Guid roomId)
        {
            var room = await FindRoom(roomId) ?? throw new StatusCodeException(StatusCode.DontExistRoom);
            var user = room.Users.FirstOrDefault(u => u.Id == userId) ?? await dataBase
                           .Users
                           .Include(u => u.UserRoomRoles)
                           .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                throw new Exception("Нет пользователя с указанным Id");
            if (!roomSecyrityManager.CanDelete(user, room))
                throw new Exception("Нет права на удаление комнаты!");

            foreach (var person in room.Users)
            {
                person.RoomId = null;
                await fileServerNotifier.SetRoom(person.Id, null);
            }
            
            dataBase.Rooms.Remove(room);
            await dataBase.SaveChangesAsync();
            dataBase.Rules.Remove(room.SecurityRule);
            await dataBase.SaveChangesAsync();
        }

        public IQueryable<Room> Rooms(Guid userId)
         => roomSecyrityManager.AccessedRooms(dataBase.Rooms, userId);

        private ApplicationUser FindUser(Guid userId)
        {
            return dataBase.Users.FirstOrDefault(u => u.Id == userId);
        }

        private Task CheckCreatingRoomParams(CreateRoomRequest createRoomInfo, ApplicationUser creater)
        {
            var errorcodes = new List<StatusCode>();

            if (creater.RoomId != null)
                errorcodes.Add(StatusCode.PersonInRoomAtAWrongTime);

            switch (errorcodes.Count)
            {
                case 0: return Task.CompletedTask;
                case 1: throw new StatusCodeException(errorcodes[0]);
                default:
                    throw new MultiStatusCodeException(errorcodes);
            }
        }

        public async Task InviteUsersByPhoneToRoom(Guid currentRoomId, List<string> phones)
        {
            var rule = (await dataBase.Rooms.Include(r => r.SecurityRule).FirstOrDefaultAsync(Room => Room.Id == currentRoomId))?.SecurityRule;
            if (rule?.PrivacyRule != PrivacyRoomType.InvationPrivate)
                throw new StatusCodeException(StatusCode.IncrorrecrTargetRoomType);
            rule.Password = string.Join('\n', rule.Password
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