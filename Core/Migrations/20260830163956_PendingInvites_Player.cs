using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Core.Migrations
{
    /// <inheritdoc />
    public partial class PendingInvites_Player : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PlayerId",
                table: "PendingInvites",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PendingInvites_PlayerId",
                table: "PendingInvites",
                column: "PlayerId");

            migrationBuilder.AddForeignKey(
                name: "FK_PendingInvites_Players_PlayerId",
                table: "PendingInvites",
                column: "PlayerId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PendingInvites_Players_PlayerId",
                table: "PendingInvites");

            migrationBuilder.DropIndex(
                name: "IX_PendingInvites_PlayerId",
                table: "PendingInvites");

            migrationBuilder.DropColumn(
                name: "PlayerId",
                table: "PendingInvites");
        }
    }
}
