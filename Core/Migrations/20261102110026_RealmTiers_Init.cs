using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Core.Migrations
{
    /// <inheritdoc />
    public partial class RealmTiers_Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RealmTiers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RenderMinimum = table.Column<int>(type: "integer", nullable: false),
                    RenderMaximum = table.Column<int>(type: "integer", nullable: false),
                    RenderDefault = table.Column<int>(type: "integer", nullable: false),
                    SimulationMinimum = table.Column<int>(type: "integer", nullable: false),
                    SimulationMaximum = table.Column<int>(type: "integer", nullable: false),
                    SimulationDefault = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RealmTiers", x => x.Id);
                });
            
            migrationBuilder.Sql(@"
INSERT INTO ""RealmTiers"" (""RenderMinimum"", ""RenderMaximum"", ""RenderDefault"", ""SimulationMinimum"", ""SimulationMaximum"", ""SimulationDefault"")
VALUES (3, 32, 10, 3, 32, 10)
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RealmTiers");
        }
    }
}
