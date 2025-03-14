﻿// <auto-generated />
using System;
using Assessment_Backend.DataLayer.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Assessment_Backend.DataLayer.Migrations
{
    [DbContext(typeof(AssessmentDbContext))]
    [Migration("20250314120514_mig_Requestlog")]
    partial class mig_Requestlog
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Assessment_Backend.DataLayer.Entities.Subs.Sub", b =>
                {
                    b.Property<int>("SubsId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("SubsId"));

                    b.Property<DateTime>("BoughtDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("PurchasedPrice")
                        .HasColumnType("int");

                    b.Property<int>("SubTypeId")
                        .HasColumnType("int");

                    b.HasKey("SubsId");

                    b.HasIndex("SubTypeId");

                    b.ToTable("Sub", "Subs");
                });

            modelBuilder.Entity("Assessment_Backend.DataLayer.Entities.Subs.SubType", b =>
                {
                    b.Property<int>("SubTypeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("SubTypeId"));

                    b.Property<int>("Count")
                        .HasColumnType("int");

                    b.Property<string>("Detail")
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)");

                    b.Property<bool>("IsDelete")
                        .HasColumnType("bit");

                    b.Property<bool>("IsFree")
                        .HasColumnType("bit");

                    b.Property<int>("Price")
                        .HasMaxLength(10)
                        .HasColumnType("int");

                    b.Property<int>("PriceDiscounted")
                        .HasMaxLength(10)
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.HasKey("SubTypeId");

                    b.ToTable("SubType", "Subs");
                });

            modelBuilder.Entity("Assessment_Backend.DataLayer.Entities.User.Grade", b =>
                {
                    b.Property<int>("GradeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("GradeId"));

                    b.Property<bool>("IsDelete")
                        .HasColumnType("bit");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("GradeId");

                    b.ToTable("Grade", "User");
                });

            modelBuilder.Entity("Assessment_Backend.DataLayer.Entities.User.Student", b =>
                {
                    b.Property<int>("StudentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("StudentId"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)");

                    b.Property<int>("GradeId")
                        .HasColumnType("int");

                    b.Property<bool>("IsDelete")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(70)
                        .HasColumnType("nvarchar(70)");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasMaxLength(11)
                        .HasColumnType("nvarchar(11)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<string>("family")
                        .IsRequired()
                        .HasMaxLength(70)
                        .HasColumnType("nvarchar(70)");

                    b.HasKey("StudentId");

                    b.HasIndex("GradeId");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("Student", "User");
                });

            modelBuilder.Entity("Assessment_Backend.DataLayer.Entities.User.Teacher", b =>
                {
                    b.Property<int>("TeacherId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("TeacherId"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)");

                    b.Property<bool>("IsDelete")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(70)
                        .HasColumnType("nvarchar(70)");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasMaxLength(11)
                        .HasColumnType("nvarchar(11)");

                    b.Property<string>("TeacherCode")
                        .IsRequired()
                        .HasMaxLength(15)
                        .HasColumnType("nvarchar(15)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<string>("family")
                        .IsRequired()
                        .HasMaxLength(70)
                        .HasColumnType("nvarchar(70)");

                    b.HasKey("TeacherId");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("Teacher", "User");
                });

            modelBuilder.Entity("Assessment_Backend.DataLayer.Entities.User.TeacherSub", b =>
                {
                    b.Property<int>("TS_Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("TS_Id"));

                    b.Property<int>("Count")
                        .HasColumnType("int");

                    b.Property<int>("SubsId")
                        .HasColumnType("int");

                    b.Property<int?>("SubsId1")
                        .HasColumnType("int");

                    b.Property<int>("TeacherId")
                        .HasColumnType("int");

                    b.HasKey("TS_Id");

                    b.HasIndex("SubsId1");

                    b.HasIndex("TeacherId");

                    b.ToTable("TeacherSub", "User");
                });

            modelBuilder.Entity("Assessment_Backend.DataLayer.Entities.User.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UserId"));

                    b.Property<string>("CodeMelli")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<byte>("Role")
                        .HasColumnType("tinyint");

                    b.HasKey("UserId");

                    b.HasIndex("CodeMelli")
                        .IsUnique();

                    b.ToTable("User", "User");
                });

            modelBuilder.Entity("Assessment_Backend.DataLayer.Entities.duty.Assessment", b =>
                {
                    b.Property<int>("AssessmentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("AssessmentId"));

                    b.Property<int>("CourseId")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<bool>("IsDelete")
                        .HasColumnType("bit");

                    b.Property<string>("PenaltyRule")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("AssessmentId");

                    b.HasIndex("CourseId");

                    b.ToTable("Assessment", "duty");
                });

            modelBuilder.Entity("Assessment_Backend.DataLayer.Entities.duty.AssignmentSubmission", b =>
                {
                    b.Property<int>("AS_Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("AS_Id"));

                    b.Property<int>("AssignmentId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("LateScore")
                        .HasColumnType("int");

                    b.Property<int>("RawScore")
                        .HasColumnType("int");

                    b.Property<DateTime?>("ReviewedDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("StudentId")
                        .HasColumnType("int");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("AS_Id");

                    b.HasIndex("AssignmentId");

                    b.HasIndex("StudentId");

                    b.ToTable("AssignmentSubmission", "duty");
                });

            modelBuilder.Entity("Assessment_Backend.DataLayer.Entities.duty.Course", b =>
                {
                    b.Property<int>("CourseId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CourseId"));

                    b.Property<int>("CountMembers")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsDelete")
                        .HasColumnType("bit");

                    b.Property<string>("Link")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("TeacherId")
                        .HasColumnType("int");

                    b.Property<int>("TermId")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("CourseId");

                    b.HasIndex("TeacherId");

                    b.HasIndex("TermId");

                    b.ToTable("Course", "duty");
                });

            modelBuilder.Entity("Assessment_Backend.DataLayer.Entities.duty.CourseEnrollment", b =>
                {
                    b.Property<int>("CE_Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CE_Id"));

                    b.Property<int>("CourseId")
                        .HasColumnType("int");

                    b.Property<DateTime>("DateTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("StudentId")
                        .HasColumnType("int");

                    b.HasKey("CE_Id");

                    b.HasIndex("CourseId");

                    b.HasIndex("StudentId");

                    b.ToTable("CourseEnrollment", "duty");
                });

            modelBuilder.Entity("Assessment_Backend.DataLayer.Entities.duty.Term", b =>
                {
                    b.Property<int>("TermId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("TermId"));

                    b.Property<bool>("IsDelete")
                        .HasColumnType("bit");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("TermId");

                    b.ToTable("Term", "duty");
                });

            modelBuilder.Entity("Assessment_Backend.DataLayer.Entities.Subs.Sub", b =>
                {
                    b.HasOne("Assessment_Backend.DataLayer.Entities.Subs.SubType", "SubType")
                        .WithMany("Subs")
                        .HasForeignKey("SubTypeId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("SubType");
                });

            modelBuilder.Entity("Assessment_Backend.DataLayer.Entities.User.Student", b =>
                {
                    b.HasOne("Assessment_Backend.DataLayer.Entities.User.Grade", "Grade")
                        .WithMany("Students")
                        .HasForeignKey("GradeId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Assessment_Backend.DataLayer.Entities.User.User", "User")
                        .WithOne("Student")
                        .HasForeignKey("Assessment_Backend.DataLayer.Entities.User.Student", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Grade");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Assessment_Backend.DataLayer.Entities.User.Teacher", b =>
                {
                    b.HasOne("Assessment_Backend.DataLayer.Entities.User.User", "User")
                        .WithOne("Teacher")
                        .HasForeignKey("Assessment_Backend.DataLayer.Entities.User.Teacher", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Assessment_Backend.DataLayer.Entities.User.TeacherSub", b =>
                {
                    b.HasOne("Assessment_Backend.DataLayer.Entities.Subs.Sub", "Sub")
                        .WithMany("TeacherSubs")
                        .HasForeignKey("SubsId1");

                    b.HasOne("Assessment_Backend.DataLayer.Entities.User.Teacher", "Teacher")
                        .WithMany()
                        .HasForeignKey("TeacherId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Sub");

                    b.Navigation("Teacher");
                });

            modelBuilder.Entity("Assessment_Backend.DataLayer.Entities.duty.Assessment", b =>
                {
                    b.HasOne("Assessment_Backend.DataLayer.Entities.duty.Course", "Course")
                        .WithMany("Assessments")
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Course");
                });

            modelBuilder.Entity("Assessment_Backend.DataLayer.Entities.duty.AssignmentSubmission", b =>
                {
                    b.HasOne("Assessment_Backend.DataLayer.Entities.duty.Assessment", "Assessment")
                        .WithMany("AssignmentSubmissions")
                        .HasForeignKey("AssignmentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Assessment_Backend.DataLayer.Entities.User.Student", "Student")
                        .WithMany("AssignmentSubmissions")
                        .HasForeignKey("StudentId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Assessment");

                    b.Navigation("Student");
                });

            modelBuilder.Entity("Assessment_Backend.DataLayer.Entities.duty.Course", b =>
                {
                    b.HasOne("Assessment_Backend.DataLayer.Entities.User.Teacher", "Teacher")
                        .WithMany()
                        .HasForeignKey("TeacherId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Assessment_Backend.DataLayer.Entities.duty.Term", "Term")
                        .WithMany("Course")
                        .HasForeignKey("TermId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Teacher");

                    b.Navigation("Term");
                });

            modelBuilder.Entity("Assessment_Backend.DataLayer.Entities.duty.CourseEnrollment", b =>
                {
                    b.HasOne("Assessment_Backend.DataLayer.Entities.duty.Course", "Course")
                        .WithMany("CourseEnrollments")
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Assessment_Backend.DataLayer.Entities.User.Student", "Student")
                        .WithMany("CourseEnrollments")
                        .HasForeignKey("StudentId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Course");

                    b.Navigation("Student");
                });

            modelBuilder.Entity("Assessment_Backend.DataLayer.Entities.Subs.Sub", b =>
                {
                    b.Navigation("TeacherSubs");
                });

            modelBuilder.Entity("Assessment_Backend.DataLayer.Entities.Subs.SubType", b =>
                {
                    b.Navigation("Subs");
                });

            modelBuilder.Entity("Assessment_Backend.DataLayer.Entities.User.Grade", b =>
                {
                    b.Navigation("Students");
                });

            modelBuilder.Entity("Assessment_Backend.DataLayer.Entities.User.Student", b =>
                {
                    b.Navigation("AssignmentSubmissions");

                    b.Navigation("CourseEnrollments");
                });

            modelBuilder.Entity("Assessment_Backend.DataLayer.Entities.User.User", b =>
                {
                    b.Navigation("Student")
                        .IsRequired();

                    b.Navigation("Teacher")
                        .IsRequired();
                });

            modelBuilder.Entity("Assessment_Backend.DataLayer.Entities.duty.Assessment", b =>
                {
                    b.Navigation("AssignmentSubmissions");
                });

            modelBuilder.Entity("Assessment_Backend.DataLayer.Entities.duty.Course", b =>
                {
                    b.Navigation("Assessments");

                    b.Navigation("CourseEnrollments");
                });

            modelBuilder.Entity("Assessment_Backend.DataLayer.Entities.duty.Term", b =>
                {
                    b.Navigation("Course");
                });
#pragma warning restore 612, 618
        }
    }
}
