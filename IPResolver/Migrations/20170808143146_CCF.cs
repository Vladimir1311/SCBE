using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace IPResolver.Migrations
{
    public partial class CCF : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CCFServises",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CCFEndPoint = table.Column<string>(nullable: true),
                    InterfaceName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CCFServises", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CCFServises");
        }
    }
}
