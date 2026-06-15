using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Core.Migrations
{
    /// <inheritdoc />
    public partial class Realms_Subscription_Relation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Realms_Subscriptions_SubscriptionId",
                table: "Realms");

            migrationBuilder.DropIndex(
                name: "IX_Realms_SubscriptionId",
                table: "Realms");

            migrationBuilder.AddColumn<int>(
                name: "RealmId",
                table: "Subscriptions",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_RealmId",
                table: "Subscriptions",
                column: "RealmId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Subscriptions_Realms_RealmId",
                table: "Subscriptions",
                column: "RealmId",
                principalTable: "Realms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subscriptions_Realms_RealmId",
                table: "Subscriptions");

            migrationBuilder.DropIndex(
                name: "IX_Subscriptions_RealmId",
                table: "Subscriptions");

            migrationBuilder.DropColumn(
                name: "RealmId",
                table: "Subscriptions");

            migrationBuilder.CreateIndex(
                name: "IX_Realms_SubscriptionId",
                table: "Realms",
                column: "SubscriptionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Realms_Subscriptions_SubscriptionId",
                table: "Realms",
                column: "SubscriptionId",
                principalTable: "Subscriptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
