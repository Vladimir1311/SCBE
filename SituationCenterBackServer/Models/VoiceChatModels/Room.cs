using Newtonsoft.Json;
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

        //TODO сделать умерщвтление комнаты после ухода пользователей
        [JsonIgnore]
        public DateTime TimeOut { get; set; }

        [JsonIgnore]
        public IEnumerable<ApplicationUser> Users => users;
        public Room(ApplicationUser creater, byte id)
        {
            users = new List<ApplicationUser> { creater };
            creater.InRoomId = lastClientId++;
            Id = id;
        }

        internal void AddUser(ApplicationUser user)
        {
            users.Add(user);
            user.InRoomId = lastClientId++;
        }

        internal void RemoveUser(ApplicationUser user)
        {
            users.Remove(user);
        }
    }
}
