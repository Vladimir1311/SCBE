using System;
using SituationCenterCore.Data;
using SituationCenterCore.Models.Rooms.Security;
namespace SituationCenterCore.Models.Rooms
{
    public class UserRoomInvite
    {
        public Guid UserId { get; set; }
        public ApplicationUser User { get; set; }

        public Guid RoomSecurityRuleId { get; set; }
        public RoomSecurityRule RoomSecurityRule { get; set; }
    }
}
