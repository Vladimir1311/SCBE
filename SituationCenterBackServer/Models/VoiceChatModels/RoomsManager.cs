﻿using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SituationCenterBackServer.Models.VoiceChatModels
{
    public class RoomsManager : IRoomManager
    {
        private byte lastRoomId = 0;
        private List<Room> rooms = new List<Room>();
        private IConnector _connector;
        private ILogger<RoomsManager> _logger;
        private ILoggerFactory _logFactory;

        public RoomsManager(IOptions<UnrealAPIConfiguration> configs,
            ILogger<RoomsManager> logger,
            ILoggerFactory logFactory,
            IConnector connector)
        {
            _connector = connector;
            _connector.OnRecieveData += _connector_OnRecieveData;
            _connector.SetBindToUser((userId, roomId) => rooms.FirstOrDefault(R => R.Id == roomId)?.Users.FirstOrDefault(U => U.InRoomId == userId));
            _connector.Start();
            _logger = logger;
            _logFactory = logFactory;
        }

        private void _connector_OnRecieveData(FromClientPack dataPack)
        {
            var targetRoom = rooms.FirstOrDefault(R => R.Users.Contains(dataPack.User));
            targetRoom?.UserSended(_connector, dataPack);
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
            return (newRoom, creater.InRoomId);
            
        }

        public IEnumerable<Room> FindRooms(Predicate<Room> func)
        {
            return rooms.Where(R => func(R));
        }

        public Room FirstOrDefault(Predicate<Room> func)
        {
            return rooms.FirstOrDefault(R => func(R));
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
            return (calledRoom,user.InRoomId);
        }

        public bool RemoveFromRoom(ApplicationUser user)
        {
            var targetRoom = rooms.FirstOrDefault(R => R.Users.Contains(user));
            if (targetRoom == null) return false;
            targetRoom.RemoveUser(user);
            return true;
        }

        public IEnumerable<Room> Rooms => rooms;
    }
}
