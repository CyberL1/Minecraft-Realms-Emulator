using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Minecraft_Realms_Emulator.Migrations
{
    /// <inheritdoc />
    public partial class Minigames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MinigameImage",
                table: "Worlds");

            migrationBuilder.DropColumn(
                name: "MinigameName",
                table: "Worlds");

            migrationBuilder.CreateIndex(
                name: "IX_Worlds_MinigameId",
                table: "Worlds",
                column: "MinigameId");

            migrationBuilder.AddForeignKey(
                name: "FK_Worlds_Templates_MinigameId",
                table: "Worlds",
                column: "MinigameId",
                principalTable: "Templates",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Worlds_Templates_MinigameId",
                table: "Worlds");

            migrationBuilder.DropIndex(
                name: "IX_Worlds_MinigameId",
                table: "Worlds");

            migrationBuilder.AddColumn<string>(
                name: "MinigameImage",
                table: "Worlds",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MinigameName",
                table: "Worlds",
                type: "text",
                nullable: true);
        }
    }
}
