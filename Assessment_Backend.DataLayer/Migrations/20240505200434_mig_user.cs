using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Assessment_Backend.DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class mig_user : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_User_RoleId",
                schema: "User",
                table: "User");

            migrationBuilder.CreateIndex(
                name: "IX_User_RoleId",
                schema: "User",
                table: "User",
                column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_User_RoleId",
                schema: "User",
                table: "User");

            migrationBuilder.CreateIndex(
                name: "IX_User_RoleId",
                schema: "User",
                table: "User",
                column: "RoleId",
                unique: true);
        }
    }
}
