using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Minecraft_Realms_Emulator.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Worlds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RemoteSubscriptionId = table.Column<string>(type: "text", nullable: true),
                    Owner = table.Column<string>(type: "text", nullable: true),
                    OwnerUUID = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Motd = table.Column<string>(type: "text", nullable: true),
                    State = table.Column<string>(type: "text", nullable: false),
                    DaysLeft = table.Column<int>(type: "integer", nullable: false),
                    Expired = table.Column<bool>(type: "boolean", nullable: false),
                    ExpiredTrial = table.Column<bool>(type: "boolean", nullable: false),
                    WorldType = table.Column<string>(type: "text", nullable: false),
                    Players = table.Column<string[]>(type: "text[]", nullable: false),
                    MaxPlayers = table.Column<int>(type: "integer", nullable: false),
                    MinigameName = table.Column<string>(type: "text", nullable: true),
                    MinigameId = table.Column<int>(type: "integer", nullable: true),
                    MinigameImage = table.Column<string>(type: "text", nullable: true),
                    ActiveSlot = table.Column<int>(type: "integer", nullable: false),
                    Slots = table.Column<string[]>(type: "text[]", nullable: false),
                    Member = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Worlds", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Worlds");
        }
    }
}
