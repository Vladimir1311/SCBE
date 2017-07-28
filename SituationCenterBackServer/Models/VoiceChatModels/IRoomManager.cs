﻿using Common.Requests.Room.CreateRoom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SituationCenterBackServer.Models.VoiceChatModels
{
    public interface IRoomManager
    {
        (Room room, byte clientId) CreateNewRoom(ApplicationUser creater, CreateRoomRequest createRoomInfo);

        (Room room, byte clientId) JoinToRoom(ApplicationUser user, Guid roomId, string securityData);

        bool RemoveFromRoom(string UserId);

        IEnumerable<Room> Rooms { get; }

        IEnumerable<Room> FindRooms(Predicate<Room> func);
        Room FindRoom(Guid roomId);

        event Action<ApplicationUser> SaveState;
    }
}
