using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Minecraft_Realms_Emulator.Migrations
{
    /// <inheritdoc />
    public partial class Slots_unused_options : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CommandBlocks",
                table: "Slots");

            migrationBuilder.DropColumn(
                name: "Pvp",
                table: "Slots");

            migrationBuilder.DropColumn(
                name: "SpawnAnimals",
                table: "Slots");

            migrationBuilder.DropColumn(
                name: "SpawnMonsters",
                table: "Slots");

            migrationBuilder.DropColumn(
                name: "SpawnNPCs",
                table: "Slots");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CommandBlocks",
                table: "Slots",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Pvp",
                table: "Slots",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SpawnAnimals",
                table: "Slots",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SpawnMonsters",
                table: "Slots",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SpawnNPCs",
                table: "Slots",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
