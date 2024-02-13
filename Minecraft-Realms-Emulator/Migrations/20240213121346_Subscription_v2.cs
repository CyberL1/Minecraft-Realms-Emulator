using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Minecraft_Realms_Emulator.Migrations
{
    /// <inheritdoc />
    public partial class Subscription_v2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RemoteId",
                table: "Subscriptions");

            migrationBuilder.RenameColumn(
                name: "DaysLeft",
                table: "Subscriptions",
                newName: "WorldId");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_WorldId",
                table: "Subscriptions",
                column: "WorldId");

            migrationBuilder.AddForeignKey(
                name: "FK_Subscriptions_Worlds_WorldId",
                table: "Subscriptions",
                column: "WorldId",
                principalTable: "Worlds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subscriptions_Worlds_WorldId",
                table: "Subscriptions");

            migrationBuilder.DropIndex(
                name: "IX_Subscriptions_WorldId",
                table: "Subscriptions");

            migrationBuilder.RenameColumn(
                name: "WorldId",
                table: "Subscriptions",
                newName: "DaysLeft");

            migrationBuilder.AddColumn<string>(
                name: "RemoteId",
                table: "Subscriptions",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
