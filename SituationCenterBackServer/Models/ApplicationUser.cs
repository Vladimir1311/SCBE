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

        [NotMapped]
        public IPEndPoint Adress { get; set; }
        [NotMapped]
        public byte InRoomId { get; set; }

        public override bool Equals(object obj)
        {
            return (obj as ApplicationUser)?.Id == Id;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
