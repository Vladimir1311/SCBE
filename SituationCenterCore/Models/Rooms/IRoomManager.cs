using Common.Requests.Room.CreateRoom;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace SituationCenterCore.Models.Rooms
{
    public interface IRoomManager
    {
        Task<Room> CreateNewRoom(Guid createrId, CreateRoomRequest createRoomInfo);

        Task JoinToRoom(Guid userId, Guid roomId, string securityData);

        Task LeaveFromRoom(Guid UserId);

        IQueryable<Room> Rooms(Guid userId);

        Task<Room> FindRoom(Guid roomId);

        Task DeleteRoom(Guid userId, Guid roomId);
        Task InviteUsersByPhoneToRoom(Guid currentRoomId, List<string> phones);
    }
}