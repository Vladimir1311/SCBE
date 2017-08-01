using Common.Exceptions;
using Common.Requests.Room.CreateRoom;
using Common.ResponseObjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SituationCenterBackServer.Data;
using SituationCenterBackServer.Models.RoomSecurity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SituationCenterBackServer.Models.VoiceChatModels
{
    public class RoomsManager : IRoomManager
    {
        private ILogger<RoomsManager> logger;
        private ApplicationDbContext dataBase;
        private readonly IRoomSecurityManager roomSecyrityManager;

        public RoomsManager(IOptions<UnrealAPIConfiguration> configs,
            ILogger<RoomsManager> logger,
            UserManager<ApplicationUser> usermanager,
            ApplicationDbContext dataBase,
            IRoomSecurityManager roomSecyrityManager)

        {
            this.logger = logger;
            this.dataBase = dataBase;
            this.roomSecyrityManager = roomSecyrityManager;
        }

        public Room CreateNewRoom(Guid createrId, CreateRoomRequest createRoomInfo)
        {
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
                case Common.Models.Rooms.PrivacyRoomType.Public:
                    roomSecyrityManager.CreatePublicRule(newRoom);
                    break;

                case Common.Models.Rooms.PrivacyRoomType.Password:
                    var password = createRoomInfo.Args["password"].ToString();
                    roomSecyrityManager.CreatePasswordRule(newRoom, password);
                    break;

                case Common.Models.Rooms.PrivacyRoomType.InvationPrivate:
                default:
                    throw new NotImplementedException("Данный функционал еще не готов");
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

        public IEnumerable<Room> Rooms => FindRooms(R => true);

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
    }
}