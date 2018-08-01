using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SituationCenterCore.Data.Migrations.Production
{
    public partial class Password : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Data",
                table: "Rules",
                newName: "Password");

            migrationBuilder.CreateTable(
                name: "UserRoomRole",
                columns: table => new
                {
                    UserId = table.Column<Guid>(nullable: false),
                    RoleId = table.Column<Guid>(nullable: false),
                    RoomId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoomRole", x => new { x.RoomId, x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRoomRole_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoomRole_Rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoomRole_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserRoomRole_RoleId",
                table: "UserRoomRole",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoomRole_UserId",
                table: "UserRoomRole",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserRoomRole");

            migrationBuilder.RenameColumn(
                name: "Password",
                table: "Rules",
                newName: "Data");
        }
    }
}
