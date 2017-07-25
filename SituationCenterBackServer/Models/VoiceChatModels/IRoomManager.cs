using Common.Requests.Room.CreateRoom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SituationCenterBackServer.Models.VoiceChatModels
{
    public interface IRoomManager
    {
        (Room room, byte clientId) CreateNewRoom(Guid createrId, CreateRoomRequest createRoomInfo);

        (Room room, byte clientId) JoinToRoom(ApplicationUser user, Guid roomId, string securityData);

        void LeaveFromRoom(Guid UserId);

        IEnumerable<Room> Rooms { get; }

        IEnumerable<Room> FindRooms(Predicate<Room> func);
        Room FindRoom(Guid roomId);

        event Action<ApplicationUser> SaveState;

        void DeleteRoom(Guid userId, Guid roomId);
    }
}
