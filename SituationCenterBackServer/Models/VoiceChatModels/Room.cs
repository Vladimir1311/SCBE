using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SituationCenterBackServer.Models.VoiceChatModels
{
    public class Room
    {
        private static byte lastClientId;

        public byte Id { get; }

        private List<ApplicationUser> users;

        public string Name { get; set; }
        public DateTime TimeOut { get; set; }

        public Room(ApplicationUser creater, byte id)
        {
            users = new List<ApplicationUser> { creater };
            creater.InRoomId = lastClientId++;
            Id = id;
        }
    }
}
