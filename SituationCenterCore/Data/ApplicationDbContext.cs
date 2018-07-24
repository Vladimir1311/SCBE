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
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
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

            ConfigureUserRoomInvite(builder);
            ConfigureUserRoomRole(builder);
        }

        private void ConfigureUserRoomInvite(ModelBuilder builder)
        {
            builder.Entity<UserRoomInvite>()
                   .HasKey(uri => new { uri.RoomSecurityRuleId, uri.UserId });

            builder.Entity<UserRoomInvite>()
                   .HasOne(uri => uri.RoomSecurityRule)
                   .WithMany(r => r.Invites)
                   .HasForeignKey(uri => uri.RoomSecurityRuleId);

            builder.Entity<UserRoomInvite>()
                   .HasOne(uri => uri.User)
                   .WithMany(u => u.Invites)
                   .HasForeignKey(uri => uri.UserId);
        }

        private void ConfigureUserRoomRole(ModelBuilder builder)
        {
            builder.Entity<UserRoomRole>()
                   .HasKey(urr => new { urr.RoomId, urr.UserId, urr.RoleId });

            builder.Entity<UserRoomRole>()
                   .HasOne(urr => urr.User)
                   .WithOne(u => u.UserRoomRole);

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
