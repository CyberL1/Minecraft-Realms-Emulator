using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Minecraft_Realms_Emulator.Migrations
{
    /// <inheritdoc />
    public partial class Backup_Slots : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Backups_Worlds_WorldId",
                table: "Backups");

            migrationBuilder.RenameColumn(
                name: "WorldId",
                table: "Backups",
                newName: "SlotId");

            migrationBuilder.RenameIndex(
                name: "IX_Backups_WorldId",
                table: "Backups",
                newName: "IX_Backups_SlotId");

            migrationBuilder.AddColumn<string>(
                name: "DownloadUrl",
                table: "Backups",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ResourcePackHash",
                table: "Backups",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResourcePackUrl",
                table: "Backups",
                type: "text",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Backups_Slots_SlotId",
                table: "Backups",
                column: "SlotId",
                principalTable: "Slots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Backups_Slots_SlotId",
                table: "Backups");

            migrationBuilder.DropColumn(
                name: "DownloadUrl",
                table: "Backups");

            migrationBuilder.DropColumn(
                name: "ResourcePackHash",
                table: "Backups");

            migrationBuilder.DropColumn(
                name: "ResourcePackUrl",
                table: "Backups");

            migrationBuilder.RenameColumn(
                name: "SlotId",
                table: "Backups",
                newName: "WorldId");

            migrationBuilder.RenameIndex(
                name: "IX_Backups_SlotId",
                table: "Backups",
                newName: "IX_Backups_WorldId");

            migrationBuilder.AddForeignKey(
                name: "FK_Backups_Worlds_WorldId",
                table: "Backups",
                column: "WorldId",
                principalTable: "Worlds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
