using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SituationCenterBackServer.Models.VoiceChatModels
{
    public interface IRoomManager
    {
        (Room room, byte clientId) CreateNewRoom(ApplicationUser creater, string name);

        (Room room, byte clientId) JoinToRoom(ApplicationUser user, string roomName);
        (Room room, byte clientId) JoinToRoom(ApplicationUser user, byte roomId);

        bool RemoveFromRoom(ApplicationUser user);

        IEnumerable<Room> Rooms { get; }

        IEnumerable<Room> FindRooms(Predicate<Room> func);


    }
}
