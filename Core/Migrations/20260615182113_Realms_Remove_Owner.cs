using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Core.Migrations
{
    /// <inheritdoc />
    public partial class Realms_Remove_Owner : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Realms_Players_OwnerId",
                table: "Realms");

            migrationBuilder.DropIndex(
                name: "IX_Realms_OwnerId",
                table: "Realms");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Realms");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OwnerId",
                table: "Realms",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Realms_OwnerId",
                table: "Realms",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Realms_Players_OwnerId",
                table: "Realms",
                column: "OwnerId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
