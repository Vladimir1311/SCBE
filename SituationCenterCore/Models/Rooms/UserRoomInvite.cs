using System;
using SituationCenterCore.Data;
namespace SituationCenterCore.Models.Rooms
{
    public class UserRoomInvite
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        public ApplicationUser User { get; set; }

        public Guid RoomId { get; set; }
        public Room Room { get; set; }
    }
}
