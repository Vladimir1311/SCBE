﻿using SituationCenterBackServer.Models.VoiceChatModels;
using System.Collections.Generic;

namespace SituationCenterBackServer.Models.RoomSecurity
{
    public interface IRoomSecurityManager
    {
        void Validate(ApplicationUser user, Room room, string data);

        void CreatePublicRule(Room room);

        void CreatePasswordRule(Room room, string pasword);

        void CreateInvationRule(Room room, IEnumerable<ApplicationUser> users);

        void AddAdminRole(ApplicationUser user, Room room);

        bool CanDelete(ApplicationUser user, Room room);

        void ClearRoles(Room room);
    }
}