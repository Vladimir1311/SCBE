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

        private void OnUserDisconnected(ApplicationUser user)
        {
            try
            {
                RemoveFromRoom(user.Id);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex.Message);
            }
        }
        public (Room room, byte clientId) CreateNewRoom(ApplicationUser creater, CreateRoomRequest createRoomInfo)
        {
            if (creater.RoomId != null)
                throw new Exception("Вы уже состоите в другой комнате!");
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
                    break;
                default:
                    break;
            }
            dataBase.Add(newRoom);
            dataBase.SaveChanges();
            newRoom.AddUser(creater);
            SaveState?.Invoke(creater);
            return (newRoom, creater.InRoomId);

        }

        public IEnumerable<Room> FindRooms(Predicate<Room> func)
        {
            return dataBase.Rooms.Include(R => R.SecurityRule).Where(R => func(R));
        }
        

        public (Room room, byte clientId) JoinToRoom(ApplicationUser user, Guid roomId, string securityData)
        {
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
            SaveState?.Invoke(user);
            return (calledRoom,user.InRoomId);
        }

        public bool RemoveFromRoom(string UserId)
        {
            var user = dataBase.Users.FirstOrDefault(U => U.Id == UserId);
            user.RoomId = null;
            dataBase.SaveChanges();
            return true;
        }

        public Room FindRoom(Guid roomId)
        {
            var room = dataBase.Rooms
                .Include(R => R.Users)
                .Include(R => R.SecurityRule)
                .FirstOrDefault(R => R.Id == roomId);
            return room;
        }

        public IEnumerable<Room> Rooms => FindRooms(R => true);
    }
}
