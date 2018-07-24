using SituationCenterCore.Data;
using SituationCenterCore.Models.Rooms.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SituationCenterCore.Models.Rooms
{
    public class Room
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int PeopleCountLimit { get; set; }
        public Guid RoomSecurityRuleId { get; set; }
        public RoomSecurityRule SecurityRule { get; set; }
        public List<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
        public List<UserRoomInvite> Invites { get; set; } = new List<UserRoomInvite>();
        public DateTime TimeOut { get; set; }
    }
}
