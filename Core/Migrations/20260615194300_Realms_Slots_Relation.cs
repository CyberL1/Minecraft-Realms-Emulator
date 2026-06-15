using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Core.Migrations
{
    /// <inheritdoc />
    public partial class Realms_Slots_Relation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Realms_Slots_ActiveSlotId",
                table: "Realms");

            migrationBuilder.DropForeignKey(
                name: "FK_Slots_Realms_RealmId",
                table: "Slots");

            migrationBuilder.DropIndex(
                name: "IX_Realms_ActiveSlotId",
                table: "Realms");

            migrationBuilder.CreateIndex(
                name: "IX_Realms_ActiveSlotId",
                table: "Realms",
                column: "ActiveSlotId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Realms_Slots_ActiveSlotId",
                table: "Realms",
                column: "ActiveSlotId",
                principalTable: "Slots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Slots_Realms_RealmId",
                table: "Slots",
                column: "RealmId",
                principalTable: "Realms",
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

            migrationBuilder.DropIndex(
                name: "IX_Realms_ActiveSlotId",
                table: "Realms");

            migrationBuilder.CreateIndex(
                name: "IX_Realms_ActiveSlotId",
                table: "Realms",
                column: "ActiveSlotId");

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
        }
    }
}
