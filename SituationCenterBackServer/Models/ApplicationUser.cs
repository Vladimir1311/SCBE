using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Net;
using System.ComponentModel.DataAnnotations.Schema;
using SituationCenterBackServer.Models.VoiceChatModels;

namespace SituationCenterBackServer.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {

        public Guid? RoomId { get; set; }
        public Room Room { get; set; }

        public string Name { get; set; }
        public string Surname { get; set; }

        public DateTime Birthday { get; set; }
        public bool Sex { get; set; }

        public override bool Equals(object obj)
        {
            return (obj as ApplicationUser)?.Id == Id;
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }


        //public List<ApplicationUser> Contacts { get; set; }
    }
}
