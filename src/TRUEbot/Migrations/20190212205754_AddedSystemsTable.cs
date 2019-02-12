using Microsoft.EntityFrameworkCore.Migrations;

namespace TRUEbot.Migrations
{
    public partial class AddedSystemsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SystemId",
                table: "Players",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Systems",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true),
                    Faction = table.Column<string>(nullable: true),
                    Level = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Systems", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Players_SystemId",
                table: "Players",
                column: "SystemId");

          
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Players_Systems_SystemId",
                table: "Players");

            migrationBuilder.DropTable(
                name: "Systems");

            migrationBuilder.DropIndex(
                name: "IX_Players_SystemId",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "SystemId",
                table: "Players");
        }
    }
}
