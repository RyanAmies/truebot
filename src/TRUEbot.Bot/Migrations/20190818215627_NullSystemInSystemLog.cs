using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TRUEbot.Bot.Migrations
{
    public partial class NullSystemInSystemLog : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {



            migrationBuilder.Sql("ALTER TABLE SystemLogs RENAME TO SystemLogs_old;");
            migrationBuilder.Sql("DROP INDEX IX_SystemLogs_PlayerId");
            
            migrationBuilder.CreateTable(
                name: "SystemLogs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PlayerId = table.Column<int>(nullable: false),
                    SystemId = table.Column<int>(nullable: true),
                    DateUpdated = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SystemLogs_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SystemLogs_Systems_SystemId",
                        column: x => x.SystemId,
                        principalTable: "Systems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SystemLogs_PlayerId",
                table: "SystemLogs",
                column: "PlayerId");

            migrationBuilder.AddForeignKey(
                name: "FK_SystemLogs_Systems_SystemId",
                table: "SystemLogs",
                column: "SystemId",
                principalTable: "Systems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);







            //migrationBuilder.DropForeignKey(
            //    name: "FK_SystemLogs_Systems_SystemId",
            //    table: "SystemLogs");

            //migrationBuilder.AlterColumn<int>(
            //    name: "SystemId",
            //    table: "SystemLogs",
            //    nullable: true,
            //    oldClrType: typeof(int));

            //migrationBuilder.AddForeignKey(
            //    name: "FK_SystemLogs_Systems_SystemId",
            //    table: "SystemLogs",
            //    column: "SystemId",
            //    principalTable: "Systems",
            //    principalColumn: "Id",
            //    onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SystemLogs_Systems_SystemId",
                table: "SystemLogs");

            migrationBuilder.AlterColumn<int>(
                name: "SystemId",
                table: "SystemLogs",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SystemLogs_Systems_SystemId",
                table: "SystemLogs",
                column: "SystemId",
                principalTable: "Systems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
