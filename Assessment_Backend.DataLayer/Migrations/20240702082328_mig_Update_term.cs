using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Assessment_Backend.DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class mig_Update_term : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Course_TermId",
                schema: "duty",
                table: "Course");

            migrationBuilder.CreateIndex(
                name: "IX_Course_TermId",
                schema: "duty",
                table: "Course",
                column: "TermId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Course_TermId",
                schema: "duty",
                table: "Course");

            migrationBuilder.CreateIndex(
                name: "IX_Course_TermId",
                schema: "duty",
                table: "Course",
                column: "TermId",
                unique: true);
        }
    }
}
