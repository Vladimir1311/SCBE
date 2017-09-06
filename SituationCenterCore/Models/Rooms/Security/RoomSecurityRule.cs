
using SituationCenter.Shared.Models.Rooms;
using System;

namespace SituationCenterCore.Models.Rooms.Security
{
    public class RoomSecurityRule
    {
        public Guid Id { get; set; }
        public PrivacyRoomType PrivacyRule { get; set; }
        public string Data { get; set; }
    }
}   