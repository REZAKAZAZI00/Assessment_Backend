using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Assessment_Backend.DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class Mig_duty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Students_Users_UserId",
                table: "Students");

            migrationBuilder.DropForeignKey(
                name: "FK_Teachers_Users_UserId",
                table: "Teachers");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Roles_RoleId",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Teachers",
                table: "Teachers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Students",
                table: "Students");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Roles",
                table: "Roles");

            migrationBuilder.EnsureSchema(
                name: "duty");

            migrationBuilder.EnsureSchema(
                name: "User");

            migrationBuilder.EnsureSchema(
                name: "Subs");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "User",
                newSchema: "User");

            migrationBuilder.RenameTable(
                name: "Teachers",
                newName: "Teacher",
                newSchema: "User");

            migrationBuilder.RenameTable(
                name: "Students",
                newName: "Student",
                newSchema: "User");

            migrationBuilder.RenameTable(
                name: "Roles",
                newName: "Role",
                newSchema: "User");

            migrationBuilder.RenameIndex(
                name: "IX_Users_RoleId",
                schema: "User",
                table: "User",
                newName: "IX_User_RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_Teachers_UserId",
                schema: "User",
                table: "Teacher",
                newName: "IX_Teacher_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Students_UserId",
                schema: "User",
                table: "Student",
                newName: "IX_Student_UserId");

            migrationBuilder.AlterColumn<string>(
                name: "Password",
                schema: "User",
                table: "User",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "CodeMelli",
                schema: "User",
                table: "User",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                schema: "User",
                table: "Teacher",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                schema: "User",
                table: "Teacher",
                type: "nvarchar(70)",
                maxLength: 70,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                schema: "User",
                table: "Teacher",
                type: "nvarchar(11)",
                maxLength: 11,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TeacherCode",
                schema: "User",
                table: "Teacher",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "family",
                schema: "User",
                table: "Teacher",
                type: "nvarchar(70)",
                maxLength: 70,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                schema: "User",
                table: "Student",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "GradeId",
                schema: "User",
                table: "Student",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                schema: "User",
                table: "Student",
                type: "nvarchar(70)",
                maxLength: 70,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                schema: "User",
                table: "Student",
                type: "nvarchar(11)",
                maxLength: 11,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "family",
                schema: "User",
                table: "Student",
                type: "nvarchar(70)",
                maxLength: 70,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                schema: "User",
                table: "Role",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_User",
                schema: "User",
                table: "User",
                column: "UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Teacher",
                schema: "User",
                table: "Teacher",
                column: "TeacherId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Student",
                schema: "User",
                table: "Student",
                column: "StudentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Role",
                schema: "User",
                table: "Role",
                column: "RoleId");

            migrationBuilder.CreateTable(
                name: "Grade",
                schema: "User",
                columns: table => new
                {
                    GradeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Grade", x => x.GradeId);
                });

            migrationBuilder.CreateTable(
                name: "SubType",
                schema: "Subs",
                columns: table => new
                {
                    SubTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Count = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Detail = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Price = table.Column<int>(type: "int", maxLength: 10, nullable: false),
                    PriceDiscounted = table.Column<int>(type: "int", maxLength: 10, nullable: false),
                    IsFree = table.Column<bool>(type: "bit", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubType", x => x.SubTypeId);
                });

            migrationBuilder.CreateTable(
                name: "Term",
                schema: "duty",
                columns: table => new
                {
                    TermId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Term", x => x.TermId);
                });

            migrationBuilder.CreateTable(
                name: "Sub",
                schema: "Subs",
                columns: table => new
                {
                    SubsId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubTypeId = table.Column<int>(type: "int", nullable: false),
                    BoughtDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PurchasedPrice = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sub", x => x.SubsId);
                    table.ForeignKey(
                        name: "FK_Sub_SubType_SubTypeId",
                        column: x => x.SubTypeId,
                        principalSchema: "Subs",
                        principalTable: "SubType",
                        principalColumn: "SubTypeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Course",
                schema: "duty",
                columns: table => new
                {
                    CourseId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TeacherId = table.Column<int>(type: "int", nullable: false),
                    TermId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Link = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CountMembers = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Course", x => x.CourseId);
                    table.ForeignKey(
                        name: "FK_Course_Teacher_TeacherId",
                        column: x => x.TeacherId,
                        principalSchema: "User",
                        principalTable: "Teacher",
                        principalColumn: "TeacherId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Course_Term_TermId",
                        column: x => x.TermId,
                        principalSchema: "duty",
                        principalTable: "Term",
                        principalColumn: "TermId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TeacherSub",
                schema: "User",
                columns: table => new
                {
                    TS_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubsId = table.Column<int>(type: "int", nullable: false),
                    TeacherId = table.Column<int>(type: "int", nullable: false),
                    Count = table.Column<int>(type: "int", nullable: false),
                    SubsId1 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeacherSub", x => x.TS_Id);
                    table.ForeignKey(
                        name: "FK_TeacherSub_Sub_SubsId1",
                        column: x => x.SubsId1,
                        principalSchema: "Subs",
                        principalTable: "Sub",
                        principalColumn: "SubsId");
                    table.ForeignKey(
                        name: "FK_TeacherSub_Teacher_TeacherId",
                        column: x => x.TeacherId,
                        principalSchema: "User",
                        principalTable: "Teacher",
                        principalColumn: "TeacherId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Assessment",
                schema: "duty",
                columns: table => new
                {
                    AssessmentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourseId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PenaltyRule = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assessment", x => x.AssessmentId);
                    table.ForeignKey(
                        name: "FK_Assessment_Course_CourseId",
                        column: x => x.CourseId,
                        principalSchema: "duty",
                        principalTable: "Course",
                        principalColumn: "CourseId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CourseEnrollment",
                schema: "duty",
                columns: table => new
                {
                    CE_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    CourseId = table.Column<int>(type: "int", nullable: false),
                    DateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseEnrollment", x => x.CE_Id);
                    table.ForeignKey(
                        name: "FK_CourseEnrollment_Course_CourseId",
                        column: x => x.CourseId,
                        principalSchema: "duty",
                        principalTable: "Course",
                        principalColumn: "CourseId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CourseEnrollment_Student_StudentId",
                        column: x => x.StudentId,
                        principalSchema: "User",
                        principalTable: "Student",
                        principalColumn: "StudentId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AssignmentSubmission",
                schema: "duty",
                columns: table => new
                {
                    AS_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    AssignmentId = table.Column<int>(type: "int", nullable: false),
                    RawScore = table.Column<int>(type: "int", nullable: false),
                    LateScore = table.Column<int>(type: "int", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AssessmentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssignmentSubmission", x => x.AS_Id);
                    table.ForeignKey(
                        name: "FK_AssignmentSubmission_Assessment_AssessmentId",
                        column: x => x.AssessmentId,
                        principalSchema: "duty",
                        principalTable: "Assessment",
                        principalColumn: "AssessmentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssignmentSubmission_Student_StudentId",
                        column: x => x.StudentId,
                        principalSchema: "User",
                        principalTable: "Student",
                        principalColumn: "StudentId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Student_GradeId",
                schema: "User",
                table: "Student",
                column: "GradeId");

            migrationBuilder.CreateIndex(
                name: "IX_Assessment_CourseId",
                schema: "duty",
                table: "Assessment",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentSubmission_AssessmentId",
                schema: "duty",
                table: "AssignmentSubmission",
                column: "AssessmentId");

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentSubmission_StudentId",
                schema: "duty",
                table: "AssignmentSubmission",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Course_TeacherId",
                schema: "duty",
                table: "Course",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_Course_TermId",
                schema: "duty",
                table: "Course",
                column: "TermId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CourseEnrollment_CourseId",
                schema: "duty",
                table: "CourseEnrollment",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseEnrollment_StudentId",
                schema: "duty",
                table: "CourseEnrollment",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Sub_SubTypeId",
                schema: "Subs",
                table: "Sub",
                column: "SubTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherSub_SubsId1",
                schema: "User",
                table: "TeacherSub",
                column: "SubsId1");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherSub_TeacherId",
                schema: "User",
                table: "TeacherSub",
                column: "TeacherId");

            migrationBuilder.AddForeignKey(
                name: "FK_Student_Grade_GradeId",
                schema: "User",
                table: "Student",
                column: "GradeId",
                principalSchema: "User",
                principalTable: "Grade",
                principalColumn: "GradeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Student_User_UserId",
                schema: "User",
                table: "Student",
                column: "UserId",
                principalSchema: "User",
                principalTable: "User",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Teacher_User_UserId",
                schema: "User",
                table: "Teacher",
                column: "UserId",
                principalSchema: "User",
                principalTable: "User",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Student_Grade_GradeId",
                schema: "User",
                table: "Student");

            migrationBuilder.DropForeignKey(
                name: "FK_Student_User_UserId",
                schema: "User",
                table: "Student");

            migrationBuilder.DropForeignKey(
                name: "FK_Teacher_User_UserId",
                schema: "User",
                table: "Teacher");

            migrationBuilder.DropForeignKey(
                name: "FK_User_Role_RoleId",
                schema: "User",
                table: "User");

            migrationBuilder.DropTable(
                name: "AssignmentSubmission",
                schema: "duty");

            migrationBuilder.DropTable(
                name: "CourseEnrollment",
                schema: "duty");

            migrationBuilder.DropTable(
                name: "Grade",
                schema: "User");

            migrationBuilder.DropTable(
                name: "TeacherSub",
                schema: "User");

            migrationBuilder.DropTable(
                name: "Assessment",
                schema: "duty");

            migrationBuilder.DropTable(
                name: "Sub",
                schema: "Subs");

            migrationBuilder.DropTable(
                name: "Course",
                schema: "duty");

            migrationBuilder.DropTable(
                name: "SubType",
                schema: "Subs");

            migrationBuilder.DropTable(
                name: "Term",
                schema: "duty");

            migrationBuilder.DropPrimaryKey(
                name: "PK_User",
                schema: "User",
                table: "User");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Teacher",
                schema: "User",
                table: "Teacher");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Student",
                schema: "User",
                table: "Student");

            migrationBuilder.DropIndex(
                name: "IX_Student_GradeId",
                schema: "User",
                table: "Student");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Role",
                schema: "User",
                table: "Role");

            migrationBuilder.DropColumn(
                name: "Email",
                schema: "User",
                table: "Teacher");

            migrationBuilder.DropColumn(
                name: "Name",
                schema: "User",
                table: "Teacher");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                schema: "User",
                table: "Teacher");

            migrationBuilder.DropColumn(
                name: "TeacherCode",
                schema: "User",
                table: "Teacher");

            migrationBuilder.DropColumn(
                name: "family",
                schema: "User",
                table: "Teacher");

            migrationBuilder.DropColumn(
                name: "Email",
                schema: "User",
                table: "Student");

            migrationBuilder.DropColumn(
                name: "GradeId",
                schema: "User",
                table: "Student");

            migrationBuilder.DropColumn(
                name: "Name",
                schema: "User",
                table: "Student");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                schema: "User",
                table: "Student");

            migrationBuilder.DropColumn(
                name: "family",
                schema: "User",
                table: "Student");

            migrationBuilder.RenameTable(
                name: "User",
                schema: "User",
                newName: "Users");

            migrationBuilder.RenameTable(
                name: "Teacher",
                schema: "User",
                newName: "Teachers");

            migrationBuilder.RenameTable(
                name: "Student",
                schema: "User",
                newName: "Students");

            migrationBuilder.RenameTable(
                name: "Role",
                schema: "User",
                newName: "Roles");

            migrationBuilder.RenameIndex(
                name: "IX_User_RoleId",
                table: "Users",
                newName: "IX_Users_RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_Teacher_UserId",
                table: "Teachers",
                newName: "IX_Teachers_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Student_UserId",
                table: "Students",
                newName: "IX_Students_UserId");

            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "CodeMelli",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Roles",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Teachers",
                table: "Teachers",
                column: "TeacherId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Students",
                table: "Students",
                column: "StudentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Roles",
                table: "Roles",
                column: "RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Users_UserId",
                table: "Students",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Teachers_Users_UserId",
                table: "Teachers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Roles_RoleId",
                table: "Users",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "RoleId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
