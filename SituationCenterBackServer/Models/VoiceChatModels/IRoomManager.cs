using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SituationCenterBackServer.Models.VoiceChatModels
{
    public interface IRoomManager
    {
        (Room, byte) CreateNewRoom(ApplicationUser creater, string name);

        IEnumerable<string> RoomNames { get; }

        IEnumerable<Room> FindRooms(Predicate<Room> func);
        Room FirstOrDefault(Predicate<Room> func);

    }
}
