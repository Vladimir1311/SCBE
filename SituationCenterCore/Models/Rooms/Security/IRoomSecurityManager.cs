using SituationCenterCore.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace SituationCenterCore.Models.Rooms.Security
{
    public interface IRoomSecurityManager
    {
        void Validate(ApplicationUser user, Room room, string data);

        void CreatePublicRule(Room room);

        void CreatePasswordRule(Room room, string pasword);

        void CreateInvationRule(Room room, ICollection<ApplicationUser> userIds);

        void AddAdminRole(ApplicationUser user, Room room);
        IQueryable<Room> AccessedRooms(IQueryable<Room> rooms, Guid userId);

        bool CanDelete(ApplicationUser user, Room room);

        bool CanJoin(ApplicationUser user, Room room);
    }
}