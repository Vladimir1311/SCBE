using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace IPResolver.Migrations
{
    public partial class RorsToRows : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ServiceRors",
                table: "ServiceRors");

            migrationBuilder.RenameTable(
                name: "ServiceRors",
                newName: "ServiseRows");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ServiseRows",
                table: "ServiseRows",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ServiseRows",
                table: "ServiseRows");

            migrationBuilder.RenameTable(
                name: "ServiseRows",
                newName: "ServiceRors");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ServiceRors",
                table: "ServiceRors",
                column: "Id");
        }
    }
}
