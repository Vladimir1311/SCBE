using Common.Requests.Room.CreateRoom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SituationCenterBackServer.Models.VoiceChatModels
{
    public interface IRoomManager
    {
        Room CreateNewRoom(Guid createrId, CreateRoomRequest createRoomInfo);

        Room JoinToRoom(Guid userId, Guid roomId, string securityData);

        void LeaveFromRoom(Guid UserId);

        IEnumerable<Room> Rooms { get; }

        IEnumerable<Room> FindRooms(Predicate<Room> func);
        Room FindRoom(Guid roomId);

        event Action<ApplicationUser> SaveState;

        void DeleteRoom(Guid userId, Guid roomId);
    }
}
