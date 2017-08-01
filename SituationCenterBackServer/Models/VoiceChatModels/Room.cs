using SituationCenterBackServer.Models.RoomSecurity;
using System;
using System.Collections.Generic;

namespace SituationCenterBackServer.Models.VoiceChatModels
{
    public class Room
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int PeopleCountLimit { get; set; }
        public Guid RoomSecurityRuleId { get; set; }
        public RoomSecurityRule SecurityRule { get; set; }
        public List<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
        public DateTime TimeOut { get; set; }
    }
}