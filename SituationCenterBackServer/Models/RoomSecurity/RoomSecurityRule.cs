using Common.Models.Rooms;
using System;

namespace SituationCenterBackServer.Models.RoomSecurity
{
    public class RoomSecurityRule
    {
        public Guid Id { get; set; }
        public PrivacyRoomType PrivacyRule { get; set; }
        public string Data { get; set; }
    }
}