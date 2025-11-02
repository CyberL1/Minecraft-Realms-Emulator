using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Minecraft_Realms_Emulator.Migrations
{
    /// <inheritdoc />
    public partial class Slots_hardcore_column : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Hardcore",
                table: "Slots",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Hardcore",
                table: "Slots");
        }
    }
}
