using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Core.Migrations
{
    /// <inheritdoc />
    public partial class Realms_Remove_SubscriptionId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubscriptionId",
                table: "Realms");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SubscriptionId",
                table: "Realms",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
