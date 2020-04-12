using Microsoft.EntityFrameworkCore.Migrations;

namespace TRUEbot.Bot.Migrations
{
    public partial class MigratePlayerLocations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE Players Set SystemId = (select ID from Systems WHERE Name = Players.Location)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
