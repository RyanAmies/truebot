using Microsoft.EntityFrameworkCore.Migrations;

namespace TRUEbot.Migrations
{
    public partial class AddNormalizedAlliance : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NormalizedAlliance",
                table: "Players",
                nullable: true);

            migrationBuilder.Sql("UPDATE Players SET NormalizedAlliance = Alliance");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NormalizedAlliance",
                table: "Players");
        }
    }
}
