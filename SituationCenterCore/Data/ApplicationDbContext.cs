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

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
            builder.Entity<UserRoomInvite>()
                   .HasKey(uri => new { uri.RoomId, uri.UserId });

            builder.Entity<UserRoomInvite>()
                   .HasOne(uri => uri.Room)
                   .WithMany(r => r.Invites)
                   .HasForeignKey(uri => uri.RoomId);

            builder.Entity<UserRoomInvite>()
                   .HasOne(uri => uri.User)
                   .WithMany(u => u.Invites)
                   .HasForeignKey(uri => uri.UserId);

        }

        public DbSet<RoomSecurityRule> Rules { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<RemovedToken> RemovedTokens { get; set; }
    }
}
