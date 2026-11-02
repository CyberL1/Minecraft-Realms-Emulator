using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Core.Migrations
{
    /// <inheritdoc />
    public partial class Realm_TierId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TierId",
                table: "Realms",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateIndex(
                name: "IX_Realms_TierId",
                table: "Realms",
                column: "TierId");

            migrationBuilder.AddForeignKey(
                name: "FK_Realms_RealmTiers_TierId",
                table: "Realms",
                column: "TierId",
                principalTable: "RealmTiers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Realms_RealmTiers_TierId",
                table: "Realms");

            migrationBuilder.DropIndex(
                name: "IX_Realms_TierId",
                table: "Realms");

            migrationBuilder.DropColumn(
                name: "TierId",
                table: "Realms");
        }
    }
}
