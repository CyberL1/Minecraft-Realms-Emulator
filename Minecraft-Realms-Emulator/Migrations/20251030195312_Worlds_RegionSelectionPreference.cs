using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Minecraft_Realms_Emulator.Migrations
{
    /// <inheritdoc />
    public partial class Worlds_RegionSelectionPreference : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RegionSelectionPreference",
                table: "Worlds",
                type: "jsonb",
                nullable: false,
                defaultValue: "{}");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RegionSelectionPreference",
                table: "Worlds");
        }
    }
}
