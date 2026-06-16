using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Core.Migrations
{
    /// <inheritdoc />
    public partial class Subscription_StartDate_Type : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DaysLeft",
                table: "Subscriptions");

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "Subscriptions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Subscriptions",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "Subscriptions");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Subscriptions");

            migrationBuilder.AddColumn<int>(
                name: "DaysLeft",
                table: "Subscriptions",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
