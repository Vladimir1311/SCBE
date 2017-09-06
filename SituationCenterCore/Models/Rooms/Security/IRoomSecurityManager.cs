using SituationCenterCore.Data;
using System;
using System.Collections.Generic;

namespace SituationCenterCore.Models.Rooms.Security
{
    public interface IRoomSecurityManager
    {
        void Validate(ApplicationUser user, Room room, string data);

        void CreatePublicRule(Room room);

        void CreatePasswordRule(Room room, string pasword);

        void CreateInvationRule(Room room, Guid[] userIds);

        void AddAdminRole(ApplicationUser user, Room room);

        bool CanDelete(ApplicationUser user, Room room);

        bool CanJoin(ApplicationUser user, Room room);

        void ClearRoles(Room room);
    }
}