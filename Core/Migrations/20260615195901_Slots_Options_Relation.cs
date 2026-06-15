using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Core.Migrations
{
    /// <inheritdoc />
    public partial class Slots_Options_Relation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Slots_SlotOptions_OptionsId",
                table: "Slots");

            migrationBuilder.DropForeignKey(
                name: "FK_Slots_WorldSettings_SettingsId",
                table: "Slots");

            migrationBuilder.DropIndex(
                name: "IX_Slots_OptionsId",
                table: "Slots");

            migrationBuilder.DropIndex(
                name: "IX_Slots_SettingsId",
                table: "Slots");

            migrationBuilder.DropColumn(
                name: "OptionsId",
                table: "Slots");

            migrationBuilder.DropColumn(
                name: "SettingsId",
                table: "Slots");

            migrationBuilder.AddColumn<int>(
                name: "SlotId",
                table: "WorldSettings",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SlotId",
                table: "SlotOptions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_WorldSettings_SlotId",
                table: "WorldSettings",
                column: "SlotId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SlotOptions_SlotId",
                table: "SlotOptions",
                column: "SlotId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SlotOptions_Slots_SlotId",
                table: "SlotOptions",
                column: "SlotId",
                principalTable: "Slots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorldSettings_Slots_SlotId",
                table: "WorldSettings",
                column: "SlotId",
                principalTable: "Slots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SlotOptions_Slots_SlotId",
                table: "SlotOptions");

            migrationBuilder.DropForeignKey(
                name: "FK_WorldSettings_Slots_SlotId",
                table: "WorldSettings");

            migrationBuilder.DropIndex(
                name: "IX_WorldSettings_SlotId",
                table: "WorldSettings");

            migrationBuilder.DropIndex(
                name: "IX_SlotOptions_SlotId",
                table: "SlotOptions");

            migrationBuilder.DropColumn(
                name: "SlotId",
                table: "WorldSettings");

            migrationBuilder.DropColumn(
                name: "SlotId",
                table: "SlotOptions");

            migrationBuilder.AddColumn<int>(
                name: "OptionsId",
                table: "Slots",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SettingsId",
                table: "Slots",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Slots_OptionsId",
                table: "Slots",
                column: "OptionsId");

            migrationBuilder.CreateIndex(
                name: "IX_Slots_SettingsId",
                table: "Slots",
                column: "SettingsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Slots_SlotOptions_OptionsId",
                table: "Slots",
                column: "OptionsId",
                principalTable: "SlotOptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Slots_WorldSettings_SettingsId",
                table: "Slots",
                column: "SettingsId",
                principalTable: "WorldSettings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
