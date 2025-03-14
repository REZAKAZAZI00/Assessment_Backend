using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Assessment_Backend.DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class mig_Requestlogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RequestLog",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MethodCode = table.Column<int>(type: "int", nullable: false),
                    RequestTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ResponseTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExecutionTimeMs = table.Column<int>(type: "int", nullable: true),
                    HttpMethod = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequestPath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QueryString = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequestBody = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResponseBody = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StatusCode = table.Column<int>(type: "int", nullable: false),
                    ExceptionMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestLog", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RequestLog");
        }
    }
}
