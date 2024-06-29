using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Minecraft_Realms_Emulator.Migrations
{
    /// <inheritdoc />
    public partial class Snapshot_Worlds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ParentWorldId",
                table: "Worlds",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Worlds_ParentWorldId",
                table: "Worlds",
                column: "ParentWorldId");

            migrationBuilder.AddForeignKey(
                name: "FK_Worlds_Worlds_ParentWorldId",
                table: "Worlds",
                column: "ParentWorldId",
                principalTable: "Worlds",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Worlds_Worlds_ParentWorldId",
                table: "Worlds");

            migrationBuilder.DropIndex(
                name: "IX_Worlds_ParentWorldId",
                table: "Worlds");

            migrationBuilder.DropColumn(
                name: "ParentWorldId",
                table: "Worlds");
        }
    }
}
