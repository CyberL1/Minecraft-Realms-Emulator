using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Minecraft_Realms_Emulator.Migrations
{
    /// <inheritdoc />
    public partial class Backups_LastModifiedDate_DateTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn("LastModifiedDate", "Backups");
            migrationBuilder.AddColumn<DateTime>("LastModifiedDate", "Backups", defaultValueSql: "now()");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn("LastModifiedDate", "Backups");
            migrationBuilder.AddColumn<long>("LastModifiedDate", "Backups");
        }
    }
}