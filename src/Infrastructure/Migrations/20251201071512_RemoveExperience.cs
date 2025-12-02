using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tawtheef.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveExperience : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobConditions_AspNetUsers_CreatedById",
                table: "JobConditions");

            migrationBuilder.DropForeignKey(
                name: "FK_JobConditions_AspNetUsers_DeletedById",
                table: "JobConditions");

            migrationBuilder.DropForeignKey(
                name: "FK_JobConditions_AspNetUsers_UpdatedById",
                table: "JobConditions");

            migrationBuilder.DropForeignKey(
                name: "FK_JobConditions_Job_JobId",
                table: "JobConditions");

            migrationBuilder.DropForeignKey(
                name: "FK_JobDegrees_AspNetUsers_CreatedById",
                table: "JobDegrees");

            migrationBuilder.DropForeignKey(
                name: "FK_JobDegrees_AspNetUsers_DeletedById",
                table: "JobDegrees");

            migrationBuilder.DropForeignKey(
                name: "FK_JobDegrees_AspNetUsers_UpdatedById",
                table: "JobDegrees");

            migrationBuilder.DropForeignKey(
                name: "FK_JobDegrees_Degree_DegreeId",
                table: "JobDegrees");

            migrationBuilder.DropForeignKey(
                name: "FK_JobDegrees_Job_JobId",
                table: "JobDegrees");

            migrationBuilder.DropForeignKey(
                name: "FK_JobSkills_AspNetUsers_CreatedById",
                table: "JobSkills");

            migrationBuilder.DropForeignKey(
                name: "FK_JobSkills_AspNetUsers_DeletedById",
                table: "JobSkills");

            migrationBuilder.DropForeignKey(
                name: "FK_JobSkills_AspNetUsers_UpdatedById",
                table: "JobSkills");

            migrationBuilder.DropForeignKey(
                name: "FK_JobSkills_Job_JobId",
                table: "JobSkills");

            migrationBuilder.DropForeignKey(
                name: "FK_Qualification_RatingGrade_RatingId",
                schema: "pro",
                table: "Qualification");

            migrationBuilder.DropForeignKey(
                name: "FK_Qualification_Resources_CertificateId",
                schema: "pro",
                table: "Qualification");

            migrationBuilder.DropForeignKey(
                name: "FK_Qualification_StudyType_StudyTypeId",
                schema: "pro",
                table: "Qualification");

            migrationBuilder.DropTable(
                name: "Achievement",
                schema: "pro");

            migrationBuilder.DropPrimaryKey(
                name: "PK_JobSkills",
                table: "JobSkills");

            migrationBuilder.DropPrimaryKey(
                name: "PK_JobDegrees",
                table: "JobDegrees");

            migrationBuilder.DropPrimaryKey(
                name: "PK_JobConditions",
                table: "JobConditions");

            migrationBuilder.RenameTable(
                name: "JobSkills",
                newName: "JobSkill",
                newSchema: "hr");

            migrationBuilder.RenameTable(
                name: "JobDegrees",
                newName: "JobDegree",
                newSchema: "hr");

            migrationBuilder.RenameTable(
                name: "JobConditions",
                newName: "JobCondition",
                newSchema: "hr");

            migrationBuilder.RenameIndex(
                name: "IX_JobSkills_UpdatedById",
                schema: "hr",
                table: "JobSkill",
                newName: "IX_JobSkill_UpdatedById");

            migrationBuilder.RenameIndex(
                name: "IX_JobSkills_JobId",
                schema: "hr",
                table: "JobSkill",
                newName: "IX_JobSkill_JobId");

            migrationBuilder.RenameIndex(
                name: "IX_JobSkills_DeletedById",
                schema: "hr",
                table: "JobSkill",
                newName: "IX_JobSkill_DeletedById");

            migrationBuilder.RenameIndex(
                name: "IX_JobSkills_CreatedById",
                schema: "hr",
                table: "JobSkill",
                newName: "IX_JobSkill_CreatedById");

            migrationBuilder.RenameIndex(
                name: "IX_JobDegrees_UpdatedById",
                schema: "hr",
                table: "JobDegree",
                newName: "IX_JobDegree_UpdatedById");

            migrationBuilder.RenameIndex(
                name: "IX_JobDegrees_JobId",
                schema: "hr",
                table: "JobDegree",
                newName: "IX_JobDegree_JobId");

            migrationBuilder.RenameIndex(
                name: "IX_JobDegrees_DeletedById",
                schema: "hr",
                table: "JobDegree",
                newName: "IX_JobDegree_DeletedById");

            migrationBuilder.RenameIndex(
                name: "IX_JobDegrees_DegreeId",
                schema: "hr",
                table: "JobDegree",
                newName: "IX_JobDegree_DegreeId");

            migrationBuilder.RenameIndex(
                name: "IX_JobDegrees_CreatedById",
                schema: "hr",
                table: "JobDegree",
                newName: "IX_JobDegree_CreatedById");

            migrationBuilder.RenameIndex(
                name: "IX_JobConditions_UpdatedById",
                schema: "hr",
                table: "JobCondition",
                newName: "IX_JobCondition_UpdatedById");

            migrationBuilder.RenameIndex(
                name: "IX_JobConditions_JobId",
                schema: "hr",
                table: "JobCondition",
                newName: "IX_JobCondition_JobId");

            migrationBuilder.RenameIndex(
                name: "IX_JobConditions_DeletedById",
                schema: "hr",
                table: "JobCondition",
                newName: "IX_JobCondition_DeletedById");

            migrationBuilder.RenameIndex(
                name: "IX_JobConditions_CreatedById",
                schema: "hr",
                table: "JobCondition",
                newName: "IX_JobCondition_CreatedById");

            migrationBuilder.AlterColumn<Guid>(
                name: "UniversityId",
                schema: "pro",
                table: "Qualification",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "SubMajorId",
                schema: "pro",
                table: "Qualification",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "StudyTypeId",
                schema: "pro",
                table: "Qualification",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "RatingId",
                schema: "pro",
                table: "Qualification",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "MajorId",
                schema: "pro",
                table: "Qualification",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<int>(
                name: "GraduationYear",
                schema: "pro",
                table: "Qualification",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<decimal>(
                name: "GPA",
                schema: "pro",
                table: "Qualification",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<Guid>(
                name: "CertificateId",
                schema: "pro",
                table: "Qualification",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "Achievements",
                schema: "pro",
                table: "Experience",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_JobSkill",
                schema: "hr",
                table: "JobSkill",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_JobDegree",
                schema: "hr",
                table: "JobDegree",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_JobCondition",
                schema: "hr",
                table: "JobCondition",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_JobCondition_AspNetUsers_CreatedById",
                schema: "hr",
                table: "JobCondition",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JobCondition_AspNetUsers_DeletedById",
                schema: "hr",
                table: "JobCondition",
                column: "DeletedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JobCondition_AspNetUsers_UpdatedById",
                schema: "hr",
                table: "JobCondition",
                column: "UpdatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JobCondition_Job_JobId",
                schema: "hr",
                table: "JobCondition",
                column: "JobId",
                principalSchema: "hr",
                principalTable: "Job",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JobDegree_AspNetUsers_CreatedById",
                schema: "hr",
                table: "JobDegree",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JobDegree_AspNetUsers_DeletedById",
                schema: "hr",
                table: "JobDegree",
                column: "DeletedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JobDegree_AspNetUsers_UpdatedById",
                schema: "hr",
                table: "JobDegree",
                column: "UpdatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JobDegree_Degree_DegreeId",
                schema: "hr",
                table: "JobDegree",
                column: "DegreeId",
                principalSchema: "lkp",
                principalTable: "Degree",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JobDegree_Job_JobId",
                schema: "hr",
                table: "JobDegree",
                column: "JobId",
                principalSchema: "hr",
                principalTable: "Job",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JobSkill_AspNetUsers_CreatedById",
                schema: "hr",
                table: "JobSkill",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JobSkill_AspNetUsers_DeletedById",
                schema: "hr",
                table: "JobSkill",
                column: "DeletedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JobSkill_AspNetUsers_UpdatedById",
                schema: "hr",
                table: "JobSkill",
                column: "UpdatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JobSkill_Job_JobId",
                schema: "hr",
                table: "JobSkill",
                column: "JobId",
                principalSchema: "hr",
                principalTable: "Job",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Qualification_RatingGrade_RatingId",
                schema: "pro",
                table: "Qualification",
                column: "RatingId",
                principalSchema: "lkp",
                principalTable: "RatingGrade",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Qualification_Resources_CertificateId",
                schema: "pro",
                table: "Qualification",
                column: "CertificateId",
                principalTable: "Resources",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Qualification_StudyType_StudyTypeId",
                schema: "pro",
                table: "Qualification",
                column: "StudyTypeId",
                principalSchema: "lkp",
                principalTable: "StudyType",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobCondition_AspNetUsers_CreatedById",
                schema: "hr",
                table: "JobCondition");

            migrationBuilder.DropForeignKey(
                name: "FK_JobCondition_AspNetUsers_DeletedById",
                schema: "hr",
                table: "JobCondition");

            migrationBuilder.DropForeignKey(
                name: "FK_JobCondition_AspNetUsers_UpdatedById",
                schema: "hr",
                table: "JobCondition");

            migrationBuilder.DropForeignKey(
                name: "FK_JobCondition_Job_JobId",
                schema: "hr",
                table: "JobCondition");

            migrationBuilder.DropForeignKey(
                name: "FK_JobDegree_AspNetUsers_CreatedById",
                schema: "hr",
                table: "JobDegree");

            migrationBuilder.DropForeignKey(
                name: "FK_JobDegree_AspNetUsers_DeletedById",
                schema: "hr",
                table: "JobDegree");

            migrationBuilder.DropForeignKey(
                name: "FK_JobDegree_AspNetUsers_UpdatedById",
                schema: "hr",
                table: "JobDegree");

            migrationBuilder.DropForeignKey(
                name: "FK_JobDegree_Degree_DegreeId",
                schema: "hr",
                table: "JobDegree");

            migrationBuilder.DropForeignKey(
                name: "FK_JobDegree_Job_JobId",
                schema: "hr",
                table: "JobDegree");

            migrationBuilder.DropForeignKey(
                name: "FK_JobSkill_AspNetUsers_CreatedById",
                schema: "hr",
                table: "JobSkill");

            migrationBuilder.DropForeignKey(
                name: "FK_JobSkill_AspNetUsers_DeletedById",
                schema: "hr",
                table: "JobSkill");

            migrationBuilder.DropForeignKey(
                name: "FK_JobSkill_AspNetUsers_UpdatedById",
                schema: "hr",
                table: "JobSkill");

            migrationBuilder.DropForeignKey(
                name: "FK_JobSkill_Job_JobId",
                schema: "hr",
                table: "JobSkill");

            migrationBuilder.DropForeignKey(
                name: "FK_Qualification_RatingGrade_RatingId",
                schema: "pro",
                table: "Qualification");

            migrationBuilder.DropForeignKey(
                name: "FK_Qualification_Resources_CertificateId",
                schema: "pro",
                table: "Qualification");

            migrationBuilder.DropForeignKey(
                name: "FK_Qualification_StudyType_StudyTypeId",
                schema: "pro",
                table: "Qualification");

            migrationBuilder.DropPrimaryKey(
                name: "PK_JobSkill",
                schema: "hr",
                table: "JobSkill");

            migrationBuilder.DropPrimaryKey(
                name: "PK_JobDegree",
                schema: "hr",
                table: "JobDegree");

            migrationBuilder.DropPrimaryKey(
                name: "PK_JobCondition",
                schema: "hr",
                table: "JobCondition");

            migrationBuilder.RenameTable(
                name: "JobSkill",
                schema: "hr",
                newName: "JobSkills");

            migrationBuilder.RenameTable(
                name: "JobDegree",
                schema: "hr",
                newName: "JobDegrees");

            migrationBuilder.RenameTable(
                name: "JobCondition",
                schema: "hr",
                newName: "JobConditions");

            migrationBuilder.RenameIndex(
                name: "IX_JobSkill_UpdatedById",
                table: "JobSkills",
                newName: "IX_JobSkills_UpdatedById");

            migrationBuilder.RenameIndex(
                name: "IX_JobSkill_JobId",
                table: "JobSkills",
                newName: "IX_JobSkills_JobId");

            migrationBuilder.RenameIndex(
                name: "IX_JobSkill_DeletedById",
                table: "JobSkills",
                newName: "IX_JobSkills_DeletedById");

            migrationBuilder.RenameIndex(
                name: "IX_JobSkill_CreatedById",
                table: "JobSkills",
                newName: "IX_JobSkills_CreatedById");

            migrationBuilder.RenameIndex(
                name: "IX_JobDegree_UpdatedById",
                table: "JobDegrees",
                newName: "IX_JobDegrees_UpdatedById");

            migrationBuilder.RenameIndex(
                name: "IX_JobDegree_JobId",
                table: "JobDegrees",
                newName: "IX_JobDegrees_JobId");

            migrationBuilder.RenameIndex(
                name: "IX_JobDegree_DeletedById",
                table: "JobDegrees",
                newName: "IX_JobDegrees_DeletedById");

            migrationBuilder.RenameIndex(
                name: "IX_JobDegree_DegreeId",
                table: "JobDegrees",
                newName: "IX_JobDegrees_DegreeId");

            migrationBuilder.RenameIndex(
                name: "IX_JobDegree_CreatedById",
                table: "JobDegrees",
                newName: "IX_JobDegrees_CreatedById");

            migrationBuilder.RenameIndex(
                name: "IX_JobCondition_UpdatedById",
                table: "JobConditions",
                newName: "IX_JobConditions_UpdatedById");

            migrationBuilder.RenameIndex(
                name: "IX_JobCondition_JobId",
                table: "JobConditions",
                newName: "IX_JobConditions_JobId");

            migrationBuilder.RenameIndex(
                name: "IX_JobCondition_DeletedById",
                table: "JobConditions",
                newName: "IX_JobConditions_DeletedById");

            migrationBuilder.RenameIndex(
                name: "IX_JobCondition_CreatedById",
                table: "JobConditions",
                newName: "IX_JobConditions_CreatedById");

            migrationBuilder.AlterColumn<Guid>(
                name: "UniversityId",
                schema: "pro",
                table: "Qualification",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "SubMajorId",
                schema: "pro",
                table: "Qualification",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "StudyTypeId",
                schema: "pro",
                table: "Qualification",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "RatingId",
                schema: "pro",
                table: "Qualification",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "MajorId",
                schema: "pro",
                table: "Qualification",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "GraduationYear",
                schema: "pro",
                table: "Qualification",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "GPA",
                schema: "pro",
                table: "Qualification",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CertificateId",
                schema: "pro",
                table: "Qualification",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Achievements",
                schema: "pro",
                table: "Experience",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_JobSkills",
                table: "JobSkills",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_JobDegrees",
                table: "JobDegrees",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_JobConditions",
                table: "JobConditions",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Achievement",
                schema: "pro",
                columns: table => new
                {
                    CreatedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DeletedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CertificateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: true),
                    Organization = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Achievement", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Achievement_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Achievement_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Achievement_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Achievement_Resources_CertificateId",
                        column: x => x.CertificateId,
                        principalTable: "Resources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Achievement_UserProfile_UserProfileId",
                        column: x => x.UserProfileId,
                        principalSchema: "app",
                        principalTable: "UserProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Achievement_CertificateId",
                schema: "pro",
                table: "Achievement",
                column: "CertificateId");

            migrationBuilder.CreateIndex(
                name: "IX_Achievement_CreatedById",
                schema: "pro",
                table: "Achievement",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Achievement_DeletedById",
                schema: "pro",
                table: "Achievement",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_Achievement_UpdatedById",
                schema: "pro",
                table: "Achievement",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Achievement_UserProfileId",
                schema: "pro",
                table: "Achievement",
                column: "UserProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_JobConditions_AspNetUsers_CreatedById",
                table: "JobConditions",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JobConditions_AspNetUsers_DeletedById",
                table: "JobConditions",
                column: "DeletedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JobConditions_AspNetUsers_UpdatedById",
                table: "JobConditions",
                column: "UpdatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JobConditions_Job_JobId",
                table: "JobConditions",
                column: "JobId",
                principalSchema: "hr",
                principalTable: "Job",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JobDegrees_AspNetUsers_CreatedById",
                table: "JobDegrees",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JobDegrees_AspNetUsers_DeletedById",
                table: "JobDegrees",
                column: "DeletedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JobDegrees_AspNetUsers_UpdatedById",
                table: "JobDegrees",
                column: "UpdatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JobDegrees_Degree_DegreeId",
                table: "JobDegrees",
                column: "DegreeId",
                principalSchema: "lkp",
                principalTable: "Degree",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JobDegrees_Job_JobId",
                table: "JobDegrees",
                column: "JobId",
                principalSchema: "hr",
                principalTable: "Job",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JobSkills_AspNetUsers_CreatedById",
                table: "JobSkills",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JobSkills_AspNetUsers_DeletedById",
                table: "JobSkills",
                column: "DeletedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JobSkills_AspNetUsers_UpdatedById",
                table: "JobSkills",
                column: "UpdatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JobSkills_Job_JobId",
                table: "JobSkills",
                column: "JobId",
                principalSchema: "hr",
                principalTable: "Job",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Qualification_RatingGrade_RatingId",
                schema: "pro",
                table: "Qualification",
                column: "RatingId",
                principalSchema: "lkp",
                principalTable: "RatingGrade",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Qualification_Resources_CertificateId",
                schema: "pro",
                table: "Qualification",
                column: "CertificateId",
                principalTable: "Resources",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Qualification_StudyType_StudyTypeId",
                schema: "pro",
                table: "Qualification",
                column: "StudyTypeId",
                principalSchema: "lkp",
                principalTable: "StudyType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
