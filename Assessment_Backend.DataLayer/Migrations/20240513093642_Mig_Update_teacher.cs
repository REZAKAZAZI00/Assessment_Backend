using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Assessment_Backend.DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class Mig_Update_teacher : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assessment_Course_CourseId",
                schema: "duty",
                table: "Assessment");

            migrationBuilder.DropForeignKey(
                name: "FK_AssignmentSubmission_Assessment_AssessmentId",
                schema: "duty",
                table: "AssignmentSubmission");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseEnrollment_Course_CourseId",
                schema: "duty",
                table: "CourseEnrollment");

            migrationBuilder.DropIndex(
                name: "IX_AssignmentSubmission_AssessmentId",
                schema: "duty",
                table: "AssignmentSubmission");

            migrationBuilder.DropColumn(
                name: "AssessmentId",
                schema: "duty",
                table: "AssignmentSubmission");

            migrationBuilder.AddColumn<bool>(
                name: "IsDelete",
                schema: "User",
                table: "Teacher",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDelete",
                schema: "User",
                table: "Student",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentSubmission_AssignmentId",
                schema: "duty",
                table: "AssignmentSubmission",
                column: "AssignmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Assessment_Course_CourseId",
                schema: "duty",
                table: "Assessment",
                column: "CourseId",
                principalSchema: "duty",
                principalTable: "Course",
                principalColumn: "CourseId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AssignmentSubmission_Assessment_AssignmentId",
                schema: "duty",
                table: "AssignmentSubmission",
                column: "AssignmentId",
                principalSchema: "duty",
                principalTable: "Assessment",
                principalColumn: "AssessmentId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseEnrollment_Course_CourseId",
                schema: "duty",
                table: "CourseEnrollment",
                column: "CourseId",
                principalSchema: "duty",
                principalTable: "Course",
                principalColumn: "CourseId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assessment_Course_CourseId",
                schema: "duty",
                table: "Assessment");

            migrationBuilder.DropForeignKey(
                name: "FK_AssignmentSubmission_Assessment_AssignmentId",
                schema: "duty",
                table: "AssignmentSubmission");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseEnrollment_Course_CourseId",
                schema: "duty",
                table: "CourseEnrollment");

            migrationBuilder.DropIndex(
                name: "IX_AssignmentSubmission_AssignmentId",
                schema: "duty",
                table: "AssignmentSubmission");

            migrationBuilder.DropColumn(
                name: "IsDelete",
                schema: "User",
                table: "Teacher");

            migrationBuilder.DropColumn(
                name: "IsDelete",
                schema: "User",
                table: "Student");

            migrationBuilder.AddColumn<int>(
                name: "AssessmentId",
                schema: "duty",
                table: "AssignmentSubmission",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentSubmission_AssessmentId",
                schema: "duty",
                table: "AssignmentSubmission",
                column: "AssessmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Assessment_Course_CourseId",
                schema: "duty",
                table: "Assessment",
                column: "CourseId",
                principalSchema: "duty",
                principalTable: "Course",
                principalColumn: "CourseId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AssignmentSubmission_Assessment_AssessmentId",
                schema: "duty",
                table: "AssignmentSubmission",
                column: "AssessmentId",
                principalSchema: "duty",
                principalTable: "Assessment",
                principalColumn: "AssessmentId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseEnrollment_Course_CourseId",
                schema: "duty",
                table: "CourseEnrollment",
                column: "CourseId",
                principalSchema: "duty",
                principalTable: "Course",
                principalColumn: "CourseId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
