using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Minecraft_Realms_Emulator.Migrations
{
    /// <inheritdoc />
    public partial class Calculate_daysLeft : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DaysLeft",
                table: "Worlds");

            migrationBuilder.Sql("ALTER TABLE \"Subscriptions\" ALTER COLUMN \"StartDate\" TYPE timestamptz USING \"StartDate\"::timestamptz");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DaysLeft",
                table: "Worlds",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql("ALTER TABLE \"Subscriptions\" ALTER COLUMN \"StartDate\" TYPE text USING \"StartDate\"::text");
        }
    }
}
