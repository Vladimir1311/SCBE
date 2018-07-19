using Microsoft.AspNetCore.Identity;
using SituationCenterCore.Models.Rooms;
using System;
using System.Collections.Generic;
using SituationCenterCore.Models.TokenAuthModels;

namespace SituationCenterCore.Data
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser<Guid>
    {
        public Guid? RoomId { get; set; }
        public Room Room { get; set; }

        public string Name { get; set; }
        public string Surname { get; set; }

        public DateTime Birthday { get; set; }
        public bool Sex { get; set; }

        public List<RefreshToken> RefreshTokens { get; set; }

        public override bool Equals(object obj)
        {
            return (obj as ApplicationUser)?.Id == Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
