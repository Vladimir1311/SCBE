using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using SituationCenterBackServer.Models.VoiceChatModels.Connectors;
using Common.Requests.Room.CreateRoom;
using SituationCenterBackServer.Data;
using SituationCenterBackServer.Models.RoomSecurity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.EntityFrameworkCore;

namespace SituationCenterBackServer.Models.VoiceChatModels
{
    public class RoomsManager : IRoomManager
    {
        
        private ILogger<RoomsManager> logger;
        private ApplicationDbContext dataBase;
        private readonly IRoomSecurityManager roomSecyrityManager;

        public event Action<ApplicationUser> SaveState;

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
        public (Room room, byte clientId) CreateNewRoom(Guid createrId, CreateRoomRequest createRoomInfo)
        {
            var creater = dataBase.Users
                .Include(U => U.Room)
                .FirstOrDefault(U => U.Id == createrId.ToString());
            if (creater.RoomId != null)
                throw new Exception("Вы уже состоите в другой комнате!");
            //TODO Искать только в видимых для пользователя комнатах
            if (dataBase.Rooms.Any(R => R.Name == createRoomInfo.Name))
                throw new Exception("Комната с таким именем уже существует!");


            Room newRoom = new Room()
            {
                Name = createRoomInfo.Name
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
            return (newRoom, creater.InRoomId);

        }

        public IEnumerable<Room> FindRooms(Predicate<Room> func)
        {
            return dataBase.Rooms.Include(R => R.SecurityRule).Where(R => func(R));
        }
        

        public (Room room, byte clientId) JoinToRoom(Guid userId, Guid roomId, string securityData)
        {
            var user = dataBase.Users
                .Include(U => U.Room)
                .FirstOrDefault(U => U.Id == userId.ToString()) ?? throw new ArgumentException($"not user with id {userId}"); 
            if (user.RoomId != null)
                throw new Exception("Вы уже состоите в другой комнате!");
            var calledRoom = dataBase
                .Rooms
                .Include(R => R.Users)
                .FirstOrDefault(R => R.Id == roomId);
            if (calledRoom == null)
                throw new Exception("Запрашиваемой комнаты не существует");
            logger.LogDebug("Validate user");
            roomSecyrityManager.Validate(user, calledRoom, securityData);
            logger.LogDebug("Validated user");
            calledRoom.AddUser(user);
            return (calledRoom,user.InRoomId);
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
            var user = room.Users.FirstOrDefault(U => U.Id  == userId.ToString())
                ?? dataBase.Users.FirstOrDefault(U => U.Id == userId.ToString());


            if (user == null)
                throw new Exception("Нет пользователя с указанным Id");
            if (!roomSecyrityManager.CanDelete(user, room))
                throw new Exception("Нет права на удаление комнаты!");

            foreach (var person in room.Users)
                person.RoomId = null;
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
    }
}
