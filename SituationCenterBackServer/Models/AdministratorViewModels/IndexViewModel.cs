using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using SituationCenterBackServer.Models.VoiceChatModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SituationCenterBackServer.Models.AdministratorViewModels
{
    public class IndexViewModel
    {
        public List<ApplicationUser> Users { get; set; }
        public List<Room> Rooms { get; set; }
        public List<IdentityRole> Roles { get; set; }
    }
}
