using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Core.Migrations
{
    /// <inheritdoc />
    public partial class Realm_OwnerId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PendingInvites_RealmId",
                table: "PendingInvites");

            migrationBuilder.AddColumn<int>(
                name: "OwnerId",
                table: "Realms",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Realms_OwnerId",
                table: "Realms",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_PendingInvites_RealmId",
                table: "PendingInvites",
                column: "RealmId");

            migrationBuilder.AddForeignKey(
                name: "FK_Realms_Players_OwnerId",
                table: "Realms",
                column: "OwnerId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Realms_Players_OwnerId",
                table: "Realms");

            migrationBuilder.DropIndex(
                name: "IX_Realms_OwnerId",
                table: "Realms");

            migrationBuilder.DropIndex(
                name: "IX_PendingInvites_RealmId",
                table: "PendingInvites");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Realms");

            migrationBuilder.CreateIndex(
                name: "IX_PendingInvites_RealmId",
                table: "PendingInvites",
                column: "RealmId",
                unique: true);
        }
    }
}
