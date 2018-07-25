using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SituationCenterCore.Models.Rooms.Security;
using SituationCenterCore.Models.Rooms;
using SituationCenterCore.Models.TokenAuthModels;
using Microsoft.AspNetCore.Identity;

namespace SituationCenterCore.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, Role, Guid>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<RoomSecurityRule> Rules { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<RemovedToken> RemovedTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            ConfigureUserRoomRole(builder);
        }


        private void ConfigureUserRoomRole(ModelBuilder builder)
        {
            builder.Entity<UserRoomRole>()
                   .HasKey(urr => new { urr.RoomId, urr.UserId, urr.RoleId });

            builder.Entity<UserRoomRole>()
                   .HasOne(urr => urr.User)
                   .WithMany(u => u.UserRoomRoles)
                   .HasForeignKey(urr => urr.UserId);

            builder.Entity<UserRoomRole>()
                   .HasOne(urr => urr.Role)
                   .WithMany(r => r.UserRoomRoles)
                   .HasForeignKey(urr => urr.RoleId);

            builder.Entity<UserRoomRole>()
                   .HasOne(urr => urr.Room)
                   .WithMany(r => r.UserRoomRoles)
                   .HasForeignKey(urr => urr.RoomId);

        }
    }
}
