
using SituationCenter.Shared.Models.Rooms;
using System;
using System.Collections.Generic;

namespace SituationCenterCore.Models.Rooms.Security
{
    public class RoomSecurityRule
    {
        public Guid Id { get; set; }
        public PrivacyRoomType PrivacyRule { get; set; }
        public string Password { get; set; }    }
}   