using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Core.Migrations
{
    /// <inheritdoc />
    public partial class Realms_Players_Relation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Players_Realms_RealmId",
                table: "Players");

            migrationBuilder.AddForeignKey(
                name: "FK_Players_Realms_RealmId",
                table: "Players",
                column: "RealmId",
                principalTable: "Realms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Players_Realms_RealmId",
                table: "Players");

            migrationBuilder.AddForeignKey(
                name: "FK_Players_Realms_RealmId",
                table: "Players",
                column: "RealmId",
                principalTable: "Realms",
                principalColumn: "Id");
        }
    }
}
