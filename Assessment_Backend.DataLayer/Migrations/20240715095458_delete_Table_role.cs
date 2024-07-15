using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Assessment_Backend.DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class delete_Table_role : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_User_Role_RoleId",
                schema: "User",
                table: "User");

            migrationBuilder.DropTable(
                name: "Role",
                schema: "User");

            migrationBuilder.DropIndex(
                name: "IX_User_RoleId",
                schema: "User",
                table: "User");

            migrationBuilder.DropColumn(
                name: "RoleId",
                schema: "User",
                table: "User");

            migrationBuilder.AddColumn<byte>(
                name: "Role",
                schema: "User",
                table: "User",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.CreateIndex(
                name: "IX_User_CodeMelli",
                schema: "User",
                table: "User",
                column: "CodeMelli",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_User_CodeMelli",
                schema: "User",
                table: "User");

            migrationBuilder.DropColumn(
                name: "Role",
                schema: "User",
                table: "User");

            migrationBuilder.AddColumn<int>(
                name: "RoleId",
                schema: "User",
                table: "User",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Role",
                schema: "User",
                columns: table => new
                {
                    RoleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.RoleId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_User_RoleId",
                schema: "User",
                table: "User",
                column: "RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_User_Role_RoleId",
                schema: "User",
                table: "User",
                column: "RoleId",
                principalSchema: "User",
                principalTable: "Role",
                principalColumn: "RoleId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
