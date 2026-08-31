using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Core.Migrations
{
    /// <inheritdoc />
    public partial class PendingInvite_RealmId_NonUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PendingInvites_RealmId",
                table: "PendingInvites");

            migrationBuilder.CreateIndex(
                name: "IX_PendingInvites_RealmId",
                table: "PendingInvites",
                column: "RealmId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PendingInvites_RealmId",
                table: "PendingInvites");

            migrationBuilder.CreateIndex(
                name: "IX_PendingInvites_RealmId",
                table: "PendingInvites",
                column: "RealmId",
                unique: true);
        }
    }
}
