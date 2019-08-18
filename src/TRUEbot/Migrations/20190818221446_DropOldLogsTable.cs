using Microsoft.EntityFrameworkCore.Migrations;

namespace TRUEbot.Migrations
{
    public partial class DropOldLogsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP TABLE SystemLogs_old;");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
