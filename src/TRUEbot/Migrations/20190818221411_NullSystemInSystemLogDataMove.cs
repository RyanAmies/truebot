using Microsoft.EntityFrameworkCore.Migrations;

namespace TRUEbot.Migrations
{
    public partial class NullSystemInSystemLogDataMove : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("INSERT INTO SystemLogs SELECT * FROM SystemLogs_old;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
