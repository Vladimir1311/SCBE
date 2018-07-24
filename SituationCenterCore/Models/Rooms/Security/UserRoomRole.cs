using System;
using SituationCenterCore.Data;
using System.Security.Policy;
namespace SituationCenterCore.Models.Rooms.Security
{
    public class UserRoomRole
    {
        public Guid UserId { get; set; }
        public ApplicationUser User { get; set; }

        public Guid RoleId { get; set; }
        public Role Role { get; set; }

        public Guid RoomId { get; set; }
        public Room Room { get; set; }
    }
}
