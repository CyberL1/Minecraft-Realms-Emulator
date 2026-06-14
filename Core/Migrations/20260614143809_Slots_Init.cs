using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Core.Migrations
{
    /// <inheritdoc />
    public partial class Slots_Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Realms_Slot_ActiveSlotId",
                table: "Realms");

            migrationBuilder.DropForeignKey(
                name: "FK_Slot_Realms_RealmId",
                table: "Slot");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Slot",
                table: "Slot");

            migrationBuilder.RenameTable(
                name: "Slot",
                newName: "Slots");

            migrationBuilder.RenameIndex(
                name: "IX_Slot_RealmId",
                table: "Slots",
                newName: "IX_Slots_RealmId");

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

            migrationBuilder.AddPrimaryKey(
                name: "PK_Slots",
                table: "Slots",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "SlotOptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SpawnProtection = table.Column<int>(type: "integer", nullable: false),
                    ForceGamemode = table.Column<bool>(type: "boolean", nullable: false),
                    Difficulty = table.Column<int>(type: "integer", nullable: false),
                    Gamemode = table.Column<int>(type: "integer", nullable: false),
                    SlotName = table.Column<string>(type: "text", nullable: false),
                    Version = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SlotOptions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WorldSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Hardcore = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorldSettings", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Slots_OptionsId",
                table: "Slots",
                column: "OptionsId");

            migrationBuilder.CreateIndex(
                name: "IX_Slots_SettingsId",
                table: "Slots",
                column: "SettingsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Realms_Slots_ActiveSlotId",
                table: "Realms",
                column: "ActiveSlotId",
                principalTable: "Slots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Slots_Realms_RealmId",
                table: "Slots",
                column: "RealmId",
                principalTable: "Realms",
                principalColumn: "Id");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Realms_Slots_ActiveSlotId",
                table: "Realms");

            migrationBuilder.DropForeignKey(
                name: "FK_Slots_Realms_RealmId",
                table: "Slots");

            migrationBuilder.DropForeignKey(
                name: "FK_Slots_SlotOptions_OptionsId",
                table: "Slots");

            migrationBuilder.DropForeignKey(
                name: "FK_Slots_WorldSettings_SettingsId",
                table: "Slots");

            migrationBuilder.DropTable(
                name: "SlotOptions");

            migrationBuilder.DropTable(
                name: "WorldSettings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Slots",
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

            migrationBuilder.RenameTable(
                name: "Slots",
                newName: "Slot");

            migrationBuilder.RenameIndex(
                name: "IX_Slots_RealmId",
                table: "Slot",
                newName: "IX_Slot_RealmId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Slot",
                table: "Slot",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Realms_Slot_ActiveSlotId",
                table: "Realms",
                column: "ActiveSlotId",
                principalTable: "Slot",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Slot_Realms_RealmId",
                table: "Slot",
                column: "RealmId",
                principalTable: "Realms",
                principalColumn: "Id");
        }
    }
}
