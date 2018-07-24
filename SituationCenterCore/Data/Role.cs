using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using SituationCenterCore.Models.Rooms.Security;

namespace SituationCenterCore.Data
{
    public class Role : IdentityRole<Guid>
    {
        public Role()
        {
        }
        public Role(string roleName) : base(roleName)
        {
                
        }
        public List<UserRoomRole> UserRoomRoles { get; set; }
    }
}
