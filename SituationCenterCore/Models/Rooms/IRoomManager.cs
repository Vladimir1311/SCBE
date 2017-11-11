﻿using Common.Requests.Room.CreateRoom;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SituationCenterCore.Models.Rooms
{
    public interface IRoomManager
    {
        Room CreateNewRoom(Guid createrId, CreateRoomRequest createRoomInfo);

        void JoinToRoom(Guid userId, Guid roomId, string securityData);

        void LeaveFromRoom(Guid UserId);

        IEnumerable<Room> Rooms(Guid userId);

        IEnumerable<Room> FindRooms(Predicate<Room> func);

        Room FindRoom(Guid roomId);

        void DeleteRoom(Guid userId, Guid roomId);
        Task InviteUsersByPhoneToRoom(Guid currentRoomId, List<string> phones);
    }
}