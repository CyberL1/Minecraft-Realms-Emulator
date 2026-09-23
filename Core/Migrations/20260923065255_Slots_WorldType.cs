using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Core.Migrations
{
    /// <inheritdoc />
    public partial class Slots_WorldType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "WorldType",
                table: "Slots",
                type: "text",
                nullable: false,
                defaultValue: "NORMAL");

            migrationBuilder.Sql(@"
INSERT INTO ""Slots"" (""WorldType"")
SELECT ""WorldType""
FROM ""Realms""
WHERE ""WorldType"" IS NULL");
            
            migrationBuilder.DropColumn(
                name: "WorldType",
                table: "Realms");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WorldType",
                table: "Slots");

            migrationBuilder.AddColumn<string>(
                name: "WorldType",
                table: "Realms",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
