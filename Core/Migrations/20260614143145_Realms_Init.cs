using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Core.Migrations
{
    /// <inheritdoc />
    public partial class Realms_Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RealmId",
                table: "Players",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Subscription",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SubscriptionId = table.Column<string>(type: "text", nullable: false),
                    DaysLeft = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscription", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Realms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SubscriptionId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    State = table.Column<string>(type: "text", nullable: false),
                    OwnerId = table.Column<int>(type: "integer", nullable: false),
                    WorldType = table.Column<string>(type: "text", nullable: false),
                    ActiveSlotId = table.Column<int>(type: "integer", nullable: false),
                    ParentWorldId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Realms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Realms_Players_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Realms_Realms_ParentWorldId",
                        column: x => x.ParentWorldId,
                        principalTable: "Realms",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Realms_Subscription_SubscriptionId",
                        column: x => x.SubscriptionId,
                        principalTable: "Subscription",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Slot",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SlotId = table.Column<int>(type: "integer", nullable: false),
                    RealmId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Slot", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Slot_Realms_RealmId",
                        column: x => x.RealmId,
                        principalTable: "Realms",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Players_RealmId",
                table: "Players",
                column: "RealmId");

            migrationBuilder.CreateIndex(
                name: "IX_Realms_ActiveSlotId",
                table: "Realms",
                column: "ActiveSlotId");

            migrationBuilder.CreateIndex(
                name: "IX_Realms_OwnerId",
                table: "Realms",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Realms_ParentWorldId",
                table: "Realms",
                column: "ParentWorldId");

            migrationBuilder.CreateIndex(
                name: "IX_Realms_SubscriptionId",
                table: "Realms",
                column: "SubscriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_Slot_RealmId",
                table: "Slot",
                column: "RealmId");

            migrationBuilder.AddForeignKey(
                name: "FK_Players_Realms_RealmId",
                table: "Players",
                column: "RealmId",
                principalTable: "Realms",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Realms_Slot_ActiveSlotId",
                table: "Realms",
                column: "ActiveSlotId",
                principalTable: "Slot",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Players_Realms_RealmId",
                table: "Players");

            migrationBuilder.DropForeignKey(
                name: "FK_Realms_Slot_ActiveSlotId",
                table: "Realms");

            migrationBuilder.DropTable(
                name: "Slot");

            migrationBuilder.DropTable(
                name: "Realms");

            migrationBuilder.DropTable(
                name: "Subscription");

            migrationBuilder.DropIndex(
                name: "IX_Players_RealmId",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "RealmId",
                table: "Players");
        }
    }
}
