using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Minecraft_Realms_Emulator.Migrations
{
    /// <inheritdoc />
    public partial class Worlds_ActiveSlot_Relacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActiveSlot",
                table: "Worlds");

            migrationBuilder.AddColumn<int>(
                name: "ActiveSlotId",
                table: "Worlds",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Worlds_ActiveSlotId",
                table: "Worlds",
                column: "ActiveSlotId");

            migrationBuilder.AddForeignKey(
                name: "FK_Worlds_Slots_ActiveSlotId",
                table: "Worlds",
                column: "ActiveSlotId",
                principalTable: "Slots",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Worlds_Slots_ActiveSlotId",
                table: "Worlds");

            migrationBuilder.DropIndex(
                name: "IX_Worlds_ActiveSlotId",
                table: "Worlds");

            migrationBuilder.DropColumn(
                name: "ActiveSlotId",
                table: "Worlds");

            migrationBuilder.AddColumn<int>(
                name: "ActiveSlot",
                table: "Worlds",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
