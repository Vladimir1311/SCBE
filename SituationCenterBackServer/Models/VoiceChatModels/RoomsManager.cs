using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using SituationCenterBackServer.Models.VoiceChatModels.Connectors;

namespace SituationCenterBackServer.Models.VoiceChatModels
{
    public class RoomsManager : IRoomManager
    {
        private byte lastRoomId = 0;
        private List<Room> rooms = new List<Room>();
        private IStableConnector _stableConnector;
        private ILogger<RoomsManager> _logger;
        private Dictionary<ApplicationUser, Room> _userToRoom = new Dictionary<ApplicationUser, Room>();
        private IConnector _nonStableConnector;

        public RoomsManager(IOptions<UnrealAPIConfiguration> configs,
            ILogger<RoomsManager> logger,
            IStableConnector stableConnector,
            IConnector nonStableConnector,
            UserManager<ApplicationUser> usermanager)
        {
            _stableConnector = stableConnector;
            _stableConnector.OnRecieveData += _connector_OnRecieveData;
            _stableConnector.OnUserConnected += _connector_OnUserConnected;
            _stableConnector.SetBindToUser(userId => usermanager.Users.FirstOrDefault(user => user.Id == userId));
            _stableConnector.Start();


            _nonStableConnector = nonStableConnector;
            _nonStableConnector.OnRecieveData += P =>
                _nonStableConnector.SendPack(new ToClientPack
                {
                    Data = P.Data,
                    User = P.User,
                    PackType = P.PackType
                });
            _nonStableConnector.SetBindToUser(userId => usermanager.Users.FirstOrDefault(user => user.Id == userId));
            _nonStableConnector.Start();

            _logger = logger;
        }

        private void _connector_OnUserConnected(ApplicationUser user)
        {
            _userToRoom[user] = null;
        }

        private void _connector_OnRecieveData(FromClientPack dataPack)
        {
            _logger.LogDebug($"Recieved {dataPack.Data.Length} bytes with {dataPack.PackType} from {dataPack.User.Email}");
            switch (dataPack.PackType)
            {
                case PackType.Voice:
                    var targetRoom = rooms.FirstOrDefault(R => R.Users.Contains(dataPack.User));
                    targetRoom?.UserSpeak(_stableConnector, dataPack.User, dataPack.Data);
                    break;
            }
        }

        public (Room room, byte clientId) CreateNewRoom(ApplicationUser creater, string name)
        {
            if (rooms.Any(R => R.Users.Any(U => U.Id == creater.Id)))
                throw new Exception("Вы уже состоите в другой комнате!");
            if (rooms.Any(R => R.Name == name))
                throw new Exception("Комната с таким именем уже существует!");
            Room newRoom = new Room(creater, lastRoomId++)
            {
                Name = name
            };
            rooms.Add(newRoom);
            _userToRoom[creater] = newRoom;
            return (newRoom, creater.InRoomId);
            
        }

        public IEnumerable<Room> FindRooms(Predicate<Room> func)
        {
            return rooms.Where(R => func(R));
        }
        

        public (Room room, byte clientId) JoinToRoom(ApplicationUser user, string roomName) =>
            JoinToRoom(user, R => R.Name == roomName);

        public (Room room, byte clientId) JoinToRoom(ApplicationUser user, byte roomId) =>
            JoinToRoom(user, R => R.Id == roomId);

        private (Room, byte clientId) JoinToRoom(ApplicationUser user, Func<Room, bool> predicate)
        {
            if (rooms.Any(R => R.Users.Any(U => U.Id == user.Id)))
                throw new Exception("Вы уже состоите в другой комнате!");
            var calledRoom = rooms.FirstOrDefault(predicate);
            if (calledRoom == null)
                throw new Exception("Запрашиваемой комнаты не существует");
            calledRoom.AddUser(user);
            _userToRoom[user] = calledRoom;
            return (calledRoom,user.InRoomId);
        }

        public bool RemoveFromRoom(string UserId)
        {
            var targetPair = _userToRoom.FirstOrDefault(Pair => Pair.Key.Id == UserId);
            if (targetPair.Key == null)
                return false;
            if (targetPair.Value == null)
                return false;
            targetPair.Value.RemoveUser(targetPair.Key);
            _userToRoom[targetPair.Key] = null;
            return true;
        }

        public IEnumerable<Room> Rooms => rooms;
    }
}
