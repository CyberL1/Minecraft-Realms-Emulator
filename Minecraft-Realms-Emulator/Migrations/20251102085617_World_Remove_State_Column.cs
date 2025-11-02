using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Minecraft_Realms_Emulator.Migrations
{
    /// <inheritdoc />
    public partial class World_Remove_State_Column : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "State",
                table: "Worlds");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "Worlds",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
