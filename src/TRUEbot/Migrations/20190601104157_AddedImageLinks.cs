using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TRUEbot.Migrations
{
    public partial class AddedImageLinks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            
            migrationBuilder.DropTable(
                name: "Kills");

            migrationBuilder.CreateTable(
                name: "Kills",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PlayerId = table.Column<int>(nullable: false),
                    KilledOn = table.Column<DateTime>(nullable: false),
                    Power = table.Column<int>(nullable: false),
                    KilledBy = table.Column<string>(nullable: false),
                    KilledByNormalised = table.Column<string>(nullable: false),
                    ImageLink = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kills", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Kills_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Kills_PlayerId",
                table: "Kills",
                column: "PlayerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageLink",
                table: "Kills");

            migrationBuilder.AlterColumn<int>(
                name: "Power",
                table: "Kills",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<string>(
                name: "KilledByNormalised",
                table: "Kills",
                nullable: true,
                oldClrType: typeof(string));
        }
    }
}
