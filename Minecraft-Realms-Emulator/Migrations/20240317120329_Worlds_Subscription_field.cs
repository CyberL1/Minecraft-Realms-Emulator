using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Minecraft_Realms_Emulator.Migrations
{
    /// <inheritdoc />
    public partial class Worlds_Subscription_field : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Subscriptions_WorldId",
                table: "Subscriptions");

            migrationBuilder.DropColumn(
                name: "DaysLeft",
                table: "Worlds");

            migrationBuilder.DropColumn(
                name: "Expired",
                table: "Worlds");

            migrationBuilder.DropColumn(
                name: "ExpiredTrial",
                table: "Worlds");

            migrationBuilder.DropColumn(
                name: "RemoteSubscriptionId",
                table: "Worlds");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_WorldId",
                table: "Subscriptions",
                column: "WorldId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Subscriptions_WorldId",
                table: "Subscriptions");

            migrationBuilder.AddColumn<int>(
                name: "DaysLeft",
                table: "Worlds",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "Expired",
                table: "Worlds",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ExpiredTrial",
                table: "Worlds",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "RemoteSubscriptionId",
                table: "Worlds",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_WorldId",
                table: "Subscriptions",
                column: "WorldId");
        }
    }
}
