using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using IPResolver.Models;

namespace IPResolver.Migrations
{
    [DbContext(typeof(ServicesContext))]
    [Migration("20170808143146_CCF")]
    partial class CCF
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.2")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Common.ResponseObjects.IPRows.ServiceRow", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ServiceType");

                    b.Property<string>("StringIP");

                    b.HasKey("Id");

                    b.ToTable("ServiseRows");
                });

            modelBuilder.Entity("IPResolver.Models.CCFService", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CCFEndPoint");

                    b.Property<string>("InterfaceName");

                    b.HasKey("Id");

                    b.ToTable("CCFServises");
                });
        }
    }
}
