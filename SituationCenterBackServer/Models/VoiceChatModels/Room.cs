﻿using Newtonsoft.Json;
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
        public List<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
        public DateTime TimeOut { get; set; }
    }
}
