using Newtonsoft.Json;
using SituationCenterBackServer.Models.RoomSecurity;
using SituationCenterBackServer.Models.VoiceChatModels.Connectors;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SituationCenterBackServer.Models.VoiceChatModels
{
    public class Room
    {

        public Guid Id { get; set; }
        public string Name { get; set; }
        public int PeopleCountLimit { get; set; }


        public Guid RoomSecurityRuleId { get; set; }
        public RoomSecurityRule SecurityRule { get; set; }
        public List<ApplicationUser> Users { get; set; }
        
        //TODO сделать умерщвтление комнаты после ухода пользователей
        [JsonIgnore]
        public DateTime TimeOut { get; set; }


        public Room()
        {
            Users = new List<ApplicationUser>();
        }
        
        internal void AddUser(ApplicationUser user)
        {
            Users.Add(user);
        }

        internal void RemoveUser(ApplicationUser user)
        {
            Users.Remove(user);
            //TODO Усли пользователей нет - начать отсчет до сметри 
        }
    }
}
