using Microsoft.EntityFrameworkCore.Migrations;

namespace TRUEbot.Migrations
{
    public partial class AddedPlayerLevel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Level",
                table: "Players",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Level",
                table: "Players");
        }
    }
}
