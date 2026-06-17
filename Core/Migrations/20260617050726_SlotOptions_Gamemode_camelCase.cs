using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Core.Migrations
{
    /// <inheritdoc />
    public partial class SlotOptions_Gamemode_camelCase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Gamemode",
                table: "SlotOptions",
                newName: "GameMode");

            migrationBuilder.RenameColumn(
                name: "ForceGamemode",
                table: "SlotOptions",
                newName: "ForceGameMode");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "GameMode",
                table: "SlotOptions",
                newName: "Gamemode");

            migrationBuilder.RenameColumn(
                name: "ForceGameMode",
                table: "SlotOptions",
                newName: "ForceGamemode");
        }
    }
}
