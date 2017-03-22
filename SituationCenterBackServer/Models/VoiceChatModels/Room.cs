using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SituationCenterBackServer.Models.VoiceChatModels
{
    public class Room
    {
        public byte Id { get; }

        private List<ApplicationUser> users;

        public string Name { get; }
        public DateTime TimeOut { get; set; }

        public Room(ApplicationUser creater, byte id)
        {
            users = new List<ApplicationUser> { creater };
            Id = id;
        }
    }
}
