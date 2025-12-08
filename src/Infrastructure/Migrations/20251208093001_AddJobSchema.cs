using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Tawtheef.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddJobSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invitations_Job_JobId",
                table: "Invitations");

            migrationBuilder.DropForeignKey(
                name: "FK_Job_Department_RequestingDepartmentId",
                schema: "hr",
                table: "Job");

            migrationBuilder.DropForeignKey(
                name: "FK_Job_JobQuota_QuotaId",
                schema: "hr",
                table: "Job");

            migrationBuilder.DropForeignKey(
                name: "FK_Job_JobStatus_StatusId",
                schema: "hr",
                table: "Job");

            migrationBuilder.DropForeignKey(
                name: "FK_Job_Sector_WorkLocationId",
                schema: "hr",
                table: "Job");

            migrationBuilder.DropIndex(
                name: "IX_Job_QuotaId",
                schema: "hr",
                table: "Job");

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "JobStatus",
                keyColumn: "Id",
                keyValue: new Guid("0b3e5e28-0c0a-4c0c-9b6b-0af1a7f0b5c8"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "JobStatus",
                keyColumn: "Id",
                keyValue: new Guid("6cfc1e9c-9b2b-4d20-b7b1-7a4b2c0a41d4"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "JobStatus",
                keyColumn: "Id",
                keyValue: new Guid("8c6d6c8c-9f68-471a-9f07-0c2b16d4e101"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "JobStatus",
                keyColumn: "Id",
                keyValue: new Guid("b1b2c364-7b10-46b6-8b4b-9d15e2ce2a23"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "Sector",
                keyColumn: "Id",
                keyValue: new Guid("1548322c-6c05-4754-a89c-19e9d2443d62"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "Sector",
                keyColumn: "Id",
                keyValue: new Guid("8fadf8df-ae7e-4d22-8cb8-f79ed9b14375"));

            migrationBuilder.DropColumn(
                name: "Order",
                schema: "hr",
                table: "JobSkill");

            migrationBuilder.DropColumn(
                name: "Text",
                schema: "hr",
                table: "JobSkill");

            migrationBuilder.DropColumn(
                name: "Order",
                schema: "hr",
                table: "JobCondition");

            migrationBuilder.DropColumn(
                name: "Text",
                schema: "hr",
                table: "JobCondition");

            migrationBuilder.DropColumn(
                name: "Benefits",
                schema: "hr",
                table: "Job");

            migrationBuilder.DropColumn(
                name: "Description",
                schema: "hr",
                table: "Job");

            migrationBuilder.DropColumn(
                name: "Title",
                schema: "hr",
                table: "Job");

            migrationBuilder.RenameColumn(
                name: "Vacancies",
                schema: "hr",
                table: "Job",
                newName: "YearsOfExperience");

            migrationBuilder.RenameColumn(
                name: "StatusId",
                schema: "hr",
                table: "Job",
                newName: "SectorId");

            migrationBuilder.RenameColumn(
                name: "RequestingDepartmentId",
                schema: "hr",
                table: "Job",
                newName: "ManagementId");

            migrationBuilder.RenameColumn(
                name: "QuotaId",
                schema: "hr",
                table: "Job",
                newName: "JobStatusId");

            migrationBuilder.RenameColumn(
                name: "Deadline",
                schema: "hr",
                table: "Job",
                newName: "ClosingDate");

            migrationBuilder.RenameIndex(
                name: "IX_Job_StatusId",
                schema: "hr",
                table: "Job",
                newName: "IX_Job_SectorId");

            migrationBuilder.RenameIndex(
                name: "IX_Job_RequestingDepartmentId",
                schema: "hr",
                table: "Job",
                newName: "IX_Job_ManagementId");

            migrationBuilder.RenameIndex(
                name: "IX_Job_IsDeleted_Deadline",
                schema: "hr",
                table: "Job",
                newName: "IX_Job_Deleted_ClosingDate");

            migrationBuilder.RenameIndex(
                name: "IX_Job_Deadline",
                schema: "hr",
                table: "Job",
                newName: "IX_Job_ClosingDate");

            migrationBuilder.AddColumn<bool>(
                name: "ShowToApplicants",
                schema: "hr",
                table: "JobSkill",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "SkillId",
                schema: "hr",
                table: "JobSkill",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "JobId",
                schema: "hr",
                table: "JobQuota",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "TextAr",
                schema: "hr",
                table: "JobCondition",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TextEn",
                schema: "hr",
                table: "JobCondition",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<Guid>(
                name: "GenderId",
                schema: "hr",
                table: "Job",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<string>(
                name: "BenefitsAr",
                schema: "hr",
                table: "Job",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BenefitsEn",
                schema: "hr",
                table: "Job",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CancelledAt",
                schema: "hr",
                table: "Job",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DepartmentId",
                schema: "hr",
                table: "Job",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "MaximumAge",
                schema: "hr",
                table: "Job",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MinimumAge",
                schema: "hr",
                table: "Job",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfVacancies",
                schema: "hr",
                table: "Job",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "OverViewAr",
                schema: "hr",
                table: "Job",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OverViewEn",
                schema: "hr",
                table: "Job",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "QualificationDescriptionAr",
                schema: "hr",
                table: "Job",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "QualificationDescriptionEn",
                schema: "hr",
                table: "Job",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SubMajorId",
                schema: "hr",
                table: "Job",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TitleAr",
                schema: "hr",
                table: "Job",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TitleEn",
                schema: "hr",
                table: "Job",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "ManagementId",
                schema: "lkp",
                table: "Department",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "ManagmentId",
                schema: "lkp",
                table: "Department",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "JobRequiredAttachment",
                schema: "hr",
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
                    JobId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TitleAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TitleEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsMandatory = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobRequiredAttachment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobRequiredAttachment_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobRequiredAttachment_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobRequiredAttachment_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobRequiredAttachment_Job_JobId",
                        column: x => x.JobId,
                        principalSchema: "hr",
                        principalTable: "Job",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "JobResponsibility",
                schema: "hr",
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
                    JobId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TextAr = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TextEn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsMandatory = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobResponsibility", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobResponsibility_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobResponsibility_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobResponsibility_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobResponsibility_Job_JobId",
                        column: x => x.JobId,
                        principalSchema: "hr",
                        principalTable: "Job",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Managment",
                schema: "lkp",
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
                    SectorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BackendName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DescriptionAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DescriptionEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Managment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Managment_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Managment_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Managment_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Managment_Sector_SectorId",
                        column: x => x.SectorId,
                        principalSchema: "lkp",
                        principalTable: "Sector",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SkillRequirementType",
                schema: "lkp",
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
                    BackendName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DescriptionAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DescriptionEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkillRequirementType", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SkillRequirementType_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SkillRequirementType_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SkillRequirementType_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Skill",
                schema: "lkp",
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
                    MajorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SkillTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SkillRequirementTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    BackendName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DescriptionAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DescriptionEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Skill", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Skill_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Skill_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Skill_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Skill_Major_MajorId",
                        column: x => x.MajorId,
                        principalSchema: "lkp",
                        principalTable: "Major",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Skill_SkillRequirementType_SkillRequirementTypeId",
                        column: x => x.SkillRequirementTypeId,
                        principalSchema: "lkp",
                        principalTable: "SkillRequirementType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Skill_SkillType_SkillTypeId",
                        column: x => x.SkillTypeId,
                        principalSchema: "lkp",
                        principalTable: "SkillType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Department",
                keyColumn: "Id",
                keyValue: new Guid("0bfb044d-9f85-4f46-a1eb-920a1f9f519a"),
                columns: new[] { "ManagementId", "ManagmentId" },
                values: new object[] { new Guid("453aa49d-8f16-a85b-9991-c8355ac8bf00"), null });

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Department",
                keyColumn: "Id",
                keyValue: new Guid("1b01eb93-b6e3-447a-8d1c-a9ce59bf5ca7"),
                columns: new[] { "ManagementId", "ManagmentId" },
                values: new object[] { new Guid("453aa49d-8f16-a85b-9991-c8355ac8bf00"), null });

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Department",
                keyColumn: "Id",
                keyValue: new Guid("2a65e4a6-ca1b-4da3-8375-299ec39a50f9"),
                columns: new[] { "ManagementId", "ManagmentId" },
                values: new object[] { new Guid("453aa49d-8f16-a85b-9991-c8355ac8bf00"), null });

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Department",
                keyColumn: "Id",
                keyValue: new Guid("48d2a180-30dc-4314-b222-92750a9d0afe"),
                columns: new[] { "ManagementId", "ManagmentId" },
                values: new object[] { new Guid("4575068a-4f0d-b6cd-8ea8-be680d8dc992"), null });

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Department",
                keyColumn: "Id",
                keyValue: new Guid("6881d9fd-7281-457a-a2e1-3cbe827622e8"),
                columns: new[] { "ManagementId", "ManagmentId" },
                values: new object[] { new Guid("453aa49d-8f16-a85b-9991-c8355ac8bf00"), null });

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Department",
                keyColumn: "Id",
                keyValue: new Guid("7e225d86-c5e5-4225-b9fc-d255a975f5d1"),
                columns: new[] { "ManagementId", "ManagmentId" },
                values: new object[] { new Guid("453aa49d-8f16-a85b-9991-c8355ac8bf00"), null });

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Department",
                keyColumn: "Id",
                keyValue: new Guid("8acd1c68-9b65-40e1-8776-ce6d7afcf542"),
                columns: new[] { "ManagementId", "ManagmentId" },
                values: new object[] { new Guid("453aa49d-8f16-a85b-9991-c8355ac8bf00"), null });

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Department",
                keyColumn: "Id",
                keyValue: new Guid("8fd80cb6-794a-4211-b8a4-139e8e026e5b"),
                columns: new[] { "ManagementId", "ManagmentId" },
                values: new object[] { new Guid("453aa49d-8f16-a85b-9991-c8355ac8bf00"), null });

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Department",
                keyColumn: "Id",
                keyValue: new Guid("91a511fb-9b43-47fd-9cd5-fc2a9ecbc32e"),
                columns: new[] { "ManagementId", "ManagmentId" },
                values: new object[] { new Guid("453aa49d-8f16-a85b-9991-c8355ac8bf00"), null });

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Department",
                keyColumn: "Id",
                keyValue: new Guid("b858ce40-a3ac-4d27-aba9-86ef62fab4fe"),
                columns: new[] { "ManagementId", "ManagmentId" },
                values: new object[] { new Guid("453aa49d-8f16-a85b-9991-c8355ac8bf00"), null });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "JobStatus",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("0d21e063-48d3-d320-4078-d85a7c2bf622"), "ReadyForAnnouncement", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "الوظيفة جاهزة للإعلان.", "Job is ready to be announced.", 7, false, "جاهزة للإعلان", "Ready For Announcement", null, null },
                    { new Guid("114dae76-bde9-3efa-2a2b-803ebd92e109"), "Closed", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "تم الوصول إلى تاريخ نهاية التقديم ولن يُسمح بإرسال الطلبات.", "Application end date has been reached; job is not accepting new applications.", 3, false, "متوقفة", "Closed", null, null },
                    { new Guid("1e3ecad5-63fa-a11c-7acb-dd4c62ef74fd"), "Published", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "تم نشر الوظيفة.", "Job is published.", 8, false, "منشورة", "Published", null, null },
                    { new Guid("5c360b07-157c-630a-254a-9c01587d80a8"), "Approved", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "تم اعتماد الوظيفة.", "Job was approved.", 6, false, "معتمدة", "Approved", null, null },
                    { new Guid("c0d95787-8505-b84f-6f50-0b461ece500d"), "Cancelled", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "تم إلغاء الوظيفة ككل ولن يتم نشرها على المنصة.", "The job has been cancelled and will not be published on the platform.", 4, false, "ملغية", "Cancelled", null, null },
                    { new Guid("c64916a6-4bd2-a4b6-afbe-c5c3b4926530"), "Draft", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "تم إنشاء الوظيفة كمسودة ولم يتم إرسالها للجمهور.", "Job is created as a draft and not visible to the public.", 1, false, "مسودة", "Draft", null, null },
                    { new Guid("e07bd71f-c466-0eea-49cc-2b13d1d9403f"), "PendingApproval", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "الوظيفة قيد الموافقة.", "Job is waiting for approval.", 5, false, "قيد الموافقة", "Pending Approval", null, null },
                    { new Guid("e0cd7b22-8948-0c37-9b15-2e5217f0c565"), "Active", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "الوظيفة جاهزة للتقديم ويمكن إرسالها للجمهور المطلوب.", "Job is open for applications and can be published to the target audience.", 2, false, "نشطة", "Active", null, null },
                    { new Guid("f2e7748a-12fe-53ac-fbdf-e989f8aa498a"), "Rejected", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "تم رفض الوظيفة.", "Job was rejected.", 9, false, "مرفوضة", "Rejected", null, null }
                });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "Sector",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("a3c9f0eb-5d6e-6c4f-0a7b-8c9d0e1a2b3c"), "PrivateEducationSector", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "قطاع التعليم الخاص", "Private Education Sector", 5, false, "التعليم الخاص", "Private Education Sector", null, null },
                    { new Guid("b4da01fc-6e7f-7d50-1b8c-9d0e1a2b3c4d"), "AssessmentSector", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "قطاع التقييم", "Assessment Sector", 6, false, "التقييم", "Assessment Sector", null, null },
                    { new Guid("c5eb12fd-7f80-8e61-2c9d-0e1a2b3c4d5e"), "SharedServicesSector", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "قطاع الخدمات المشتركة", "Shared Services Sector", 7, false, "الخدمات المشتركة", "Shared Services Sector", null, null },
                    { new Guid("e1a7f8d9-3b4c-4a2d-8e5f-6a7b8c9d0e1f"), "DeputyMinisterSector", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "قطاع سعادة الوكيل", "Deputy Minister Sector", 3, false, "سعادة الوكيل", "Deputy Minister Sector", null, null },
                    { new Guid("f2b8e9fa-4c5d-5b3e-9f6a-7b8c9d0e1a2b"), "GeneralEducationSector", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "قطاع التعليم العام", "General Education Sector", 4, false, "التعليم العام", "General Education Sector", null, null }
                });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "SkillRequirementType",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("a014fa55-b3ed-14e4-b4aa-15354a5d1cc3"), "Optional", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "متطلب مهارة اختياري", "Optional skill requirement", 2, false, "اختياري", "Optional", null, null },
                    { new Guid("b0f1f4dd-95ad-3d0b-e74c-684d2d288329"), "Essential", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "متطلب مهارة أساسي", "Essential skill requirement", 1, false, "أساسي", "Essential", null, null }
                });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "Managment",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsDeleted", "NameAr", "NameEn", "SectorId", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("453aa49d-8f16-a85b-9991-c8355ac8bf00"), "TrainingCenter", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 2, false, "مركز التدريب", "Training Center", new Guid("f2b8e9fa-4c5d-5b3e-9f6a-7b8c9d0e1a2b"), null, null },
                    { new Guid("4575068a-4f0d-b6cd-8ea8-be680d8dc992"), "Minister", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 1, false, "الوزير", "Minister", new Guid("e1a7f8d9-3b4c-4a2d-8e5f-6a7b8c9d0e1f"), null, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_JobSkill_SkillId",
                schema: "hr",
                table: "JobSkill",
                column: "SkillId");

            migrationBuilder.CreateIndex(
                name: "IX_JobQuota_JobId",
                schema: "hr",
                table: "JobQuota",
                column: "JobId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Job_Deleted_Status",
                schema: "hr",
                table: "Job",
                columns: new[] { "IsDeleted", "JobStatusId" });

            migrationBuilder.CreateIndex(
                name: "IX_Job_DepartmentId",
                schema: "hr",
                table: "Job",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Job_JobStatusId",
                schema: "hr",
                table: "Job",
                column: "JobStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Job_Status_ClosingDate_Deleted",
                schema: "hr",
                table: "Job",
                columns: new[] { "JobStatusId", "ClosingDate", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_Job_SubMajorId",
                schema: "hr",
                table: "Job",
                column: "SubMajorId");

            migrationBuilder.CreateIndex(
                name: "IX_Job_Unique_Title_Department_Category_SubMajor",
                schema: "hr",
                table: "Job",
                columns: new[] { "TitleAr", "DepartmentId", "JobCategoryId", "SubMajorId" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Department_ManagementId",
                schema: "lkp",
                table: "Department",
                column: "ManagementId");

            migrationBuilder.CreateIndex(
                name: "IX_Department_ManagmentId",
                schema: "lkp",
                table: "Department",
                column: "ManagmentId");

            migrationBuilder.CreateIndex(
                name: "IX_JobRequiredAttachment_CreatedById",
                schema: "hr",
                table: "JobRequiredAttachment",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_JobRequiredAttachment_DeletedById",
                schema: "hr",
                table: "JobRequiredAttachment",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_JobRequiredAttachment_JobId",
                schema: "hr",
                table: "JobRequiredAttachment",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_JobRequiredAttachment_UpdatedById",
                schema: "hr",
                table: "JobRequiredAttachment",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_JobResponsibility_CreatedById",
                schema: "hr",
                table: "JobResponsibility",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_JobResponsibility_DeletedById",
                schema: "hr",
                table: "JobResponsibility",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_JobResponsibility_JobId",
                schema: "hr",
                table: "JobResponsibility",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_JobResponsibility_UpdatedById",
                schema: "hr",
                table: "JobResponsibility",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Managment_BackendName",
                schema: "lkp",
                table: "Managment",
                column: "BackendName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Managment_CreatedById",
                schema: "lkp",
                table: "Managment",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Managment_DeletedById",
                schema: "lkp",
                table: "Managment",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_Managment_DisplayOrder",
                schema: "lkp",
                table: "Managment",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_Managment_SectorId",
                schema: "lkp",
                table: "Managment",
                column: "SectorId");

            migrationBuilder.CreateIndex(
                name: "IX_Managment_UpdatedById",
                schema: "lkp",
                table: "Managment",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Skill_CreatedById",
                schema: "lkp",
                table: "Skill",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Skill_DeletedById",
                schema: "lkp",
                table: "Skill",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_Skill_MajorId",
                schema: "lkp",
                table: "Skill",
                column: "MajorId");

            migrationBuilder.CreateIndex(
                name: "IX_Skill_SkillRequirementTypeId",
                schema: "lkp",
                table: "Skill",
                column: "SkillRequirementTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Skill_SkillTypeId",
                schema: "lkp",
                table: "Skill",
                column: "SkillTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Skill_UpdatedById",
                schema: "lkp",
                table: "Skill",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_SkillRequirementType_BackendName",
                schema: "lkp",
                table: "SkillRequirementType",
                column: "BackendName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SkillRequirementType_CreatedById",
                schema: "lkp",
                table: "SkillRequirementType",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_SkillRequirementType_DeletedById",
                schema: "lkp",
                table: "SkillRequirementType",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_SkillRequirementType_DisplayOrder",
                schema: "lkp",
                table: "SkillRequirementType",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_SkillRequirementType_UpdatedById",
                schema: "lkp",
                table: "SkillRequirementType",
                column: "UpdatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Department_Managment_ManagementId",
                schema: "lkp",
                table: "Department",
                column: "ManagementId",
                principalSchema: "lkp",
                principalTable: "Managment",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Department_Managment_ManagmentId",
                schema: "lkp",
                table: "Department",
                column: "ManagmentId",
                principalSchema: "lkp",
                principalTable: "Managment",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Invitations_Job_JobId",
                table: "Invitations",
                column: "JobId",
                principalSchema: "hr",
                principalTable: "Job",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Job_Department_DepartmentId",
                schema: "hr",
                table: "Job",
                column: "DepartmentId",
                principalSchema: "lkp",
                principalTable: "Department",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Job_JobStatus_JobStatusId",
                schema: "hr",
                table: "Job",
                column: "JobStatusId",
                principalSchema: "lkp",
                principalTable: "JobStatus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Job_Major_SubMajorId",
                schema: "hr",
                table: "Job",
                column: "SubMajorId",
                principalSchema: "lkp",
                principalTable: "Major",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Job_Managment_ManagementId",
                schema: "hr",
                table: "Job",
                column: "ManagementId",
                principalSchema: "lkp",
                principalTable: "Managment",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Job_Sector_SectorId",
                schema: "hr",
                table: "Job",
                column: "SectorId",
                principalSchema: "lkp",
                principalTable: "Sector",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Job_TargetEntity_WorkLocationId",
                schema: "hr",
                table: "Job",
                column: "WorkLocationId",
                principalSchema: "lkp",
                principalTable: "TargetEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JobQuota_Job_JobId",
                schema: "hr",
                table: "JobQuota",
                column: "JobId",
                principalSchema: "hr",
                principalTable: "Job",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JobSkill_Skill_SkillId",
                schema: "hr",
                table: "JobSkill",
                column: "SkillId",
                principalSchema: "lkp",
                principalTable: "Skill",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Department_Managment_ManagementId",
                schema: "lkp",
                table: "Department");

            migrationBuilder.DropForeignKey(
                name: "FK_Department_Managment_ManagmentId",
                schema: "lkp",
                table: "Department");

            migrationBuilder.DropForeignKey(
                name: "FK_Invitations_Job_JobId",
                table: "Invitations");

            migrationBuilder.DropForeignKey(
                name: "FK_Job_Department_DepartmentId",
                schema: "hr",
                table: "Job");

            migrationBuilder.DropForeignKey(
                name: "FK_Job_JobStatus_JobStatusId",
                schema: "hr",
                table: "Job");

            migrationBuilder.DropForeignKey(
                name: "FK_Job_Major_SubMajorId",
                schema: "hr",
                table: "Job");

            migrationBuilder.DropForeignKey(
                name: "FK_Job_Managment_ManagementId",
                schema: "hr",
                table: "Job");

            migrationBuilder.DropForeignKey(
                name: "FK_Job_Sector_SectorId",
                schema: "hr",
                table: "Job");

            migrationBuilder.DropForeignKey(
                name: "FK_Job_TargetEntity_WorkLocationId",
                schema: "hr",
                table: "Job");

            migrationBuilder.DropForeignKey(
                name: "FK_JobQuota_Job_JobId",
                schema: "hr",
                table: "JobQuota");

            migrationBuilder.DropForeignKey(
                name: "FK_JobSkill_Skill_SkillId",
                schema: "hr",
                table: "JobSkill");

            migrationBuilder.DropTable(
                name: "JobRequiredAttachment",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "JobResponsibility",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "Managment",
                schema: "lkp");

            migrationBuilder.DropTable(
                name: "Skill",
                schema: "lkp");

            migrationBuilder.DropTable(
                name: "SkillRequirementType",
                schema: "lkp");

            migrationBuilder.DropIndex(
                name: "IX_JobSkill_SkillId",
                schema: "hr",
                table: "JobSkill");

            migrationBuilder.DropIndex(
                name: "IX_JobQuota_JobId",
                schema: "hr",
                table: "JobQuota");

            migrationBuilder.DropIndex(
                name: "IX_Job_Deleted_Status",
                schema: "hr",
                table: "Job");

            migrationBuilder.DropIndex(
                name: "IX_Job_DepartmentId",
                schema: "hr",
                table: "Job");

            migrationBuilder.DropIndex(
                name: "IX_Job_JobStatusId",
                schema: "hr",
                table: "Job");

            migrationBuilder.DropIndex(
                name: "IX_Job_Status_ClosingDate_Deleted",
                schema: "hr",
                table: "Job");

            migrationBuilder.DropIndex(
                name: "IX_Job_SubMajorId",
                schema: "hr",
                table: "Job");

            migrationBuilder.DropIndex(
                name: "IX_Job_Unique_Title_Department_Category_SubMajor",
                schema: "hr",
                table: "Job");

            migrationBuilder.DropIndex(
                name: "IX_Department_ManagementId",
                schema: "lkp",
                table: "Department");

            migrationBuilder.DropIndex(
                name: "IX_Department_ManagmentId",
                schema: "lkp",
                table: "Department");

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "JobStatus",
                keyColumn: "Id",
                keyValue: new Guid("0d21e063-48d3-d320-4078-d85a7c2bf622"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "JobStatus",
                keyColumn: "Id",
                keyValue: new Guid("114dae76-bde9-3efa-2a2b-803ebd92e109"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "JobStatus",
                keyColumn: "Id",
                keyValue: new Guid("1e3ecad5-63fa-a11c-7acb-dd4c62ef74fd"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "JobStatus",
                keyColumn: "Id",
                keyValue: new Guid("5c360b07-157c-630a-254a-9c01587d80a8"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "JobStatus",
                keyColumn: "Id",
                keyValue: new Guid("c0d95787-8505-b84f-6f50-0b461ece500d"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "JobStatus",
                keyColumn: "Id",
                keyValue: new Guid("c64916a6-4bd2-a4b6-afbe-c5c3b4926530"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "JobStatus",
                keyColumn: "Id",
                keyValue: new Guid("e07bd71f-c466-0eea-49cc-2b13d1d9403f"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "JobStatus",
                keyColumn: "Id",
                keyValue: new Guid("e0cd7b22-8948-0c37-9b15-2e5217f0c565"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "JobStatus",
                keyColumn: "Id",
                keyValue: new Guid("f2e7748a-12fe-53ac-fbdf-e989f8aa498a"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "Sector",
                keyColumn: "Id",
                keyValue: new Guid("a3c9f0eb-5d6e-6c4f-0a7b-8c9d0e1a2b3c"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "Sector",
                keyColumn: "Id",
                keyValue: new Guid("b4da01fc-6e7f-7d50-1b8c-9d0e1a2b3c4d"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "Sector",
                keyColumn: "Id",
                keyValue: new Guid("c5eb12fd-7f80-8e61-2c9d-0e1a2b3c4d5e"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "Sector",
                keyColumn: "Id",
                keyValue: new Guid("e1a7f8d9-3b4c-4a2d-8e5f-6a7b8c9d0e1f"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "Sector",
                keyColumn: "Id",
                keyValue: new Guid("f2b8e9fa-4c5d-5b3e-9f6a-7b8c9d0e1a2b"));

            migrationBuilder.DropColumn(
                name: "ShowToApplicants",
                schema: "hr",
                table: "JobSkill");

            migrationBuilder.DropColumn(
                name: "SkillId",
                schema: "hr",
                table: "JobSkill");

            migrationBuilder.DropColumn(
                name: "JobId",
                schema: "hr",
                table: "JobQuota");

            migrationBuilder.DropColumn(
                name: "TextAr",
                schema: "hr",
                table: "JobCondition");

            migrationBuilder.DropColumn(
                name: "TextEn",
                schema: "hr",
                table: "JobCondition");

            migrationBuilder.DropColumn(
                name: "BenefitsAr",
                schema: "hr",
                table: "Job");

            migrationBuilder.DropColumn(
                name: "BenefitsEn",
                schema: "hr",
                table: "Job");

            migrationBuilder.DropColumn(
                name: "CancelledAt",
                schema: "hr",
                table: "Job");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                schema: "hr",
                table: "Job");

            migrationBuilder.DropColumn(
                name: "MaximumAge",
                schema: "hr",
                table: "Job");

            migrationBuilder.DropColumn(
                name: "MinimumAge",
                schema: "hr",
                table: "Job");

            migrationBuilder.DropColumn(
                name: "NumberOfVacancies",
                schema: "hr",
                table: "Job");

            migrationBuilder.DropColumn(
                name: "OverViewAr",
                schema: "hr",
                table: "Job");

            migrationBuilder.DropColumn(
                name: "OverViewEn",
                schema: "hr",
                table: "Job");

            migrationBuilder.DropColumn(
                name: "QualificationDescriptionAr",
                schema: "hr",
                table: "Job");

            migrationBuilder.DropColumn(
                name: "QualificationDescriptionEn",
                schema: "hr",
                table: "Job");

            migrationBuilder.DropColumn(
                name: "SubMajorId",
                schema: "hr",
                table: "Job");

            migrationBuilder.DropColumn(
                name: "TitleAr",
                schema: "hr",
                table: "Job");

            migrationBuilder.DropColumn(
                name: "TitleEn",
                schema: "hr",
                table: "Job");

            migrationBuilder.DropColumn(
                name: "ManagementId",
                schema: "lkp",
                table: "Department");

            migrationBuilder.DropColumn(
                name: "ManagmentId",
                schema: "lkp",
                table: "Department");

            migrationBuilder.RenameColumn(
                name: "YearsOfExperience",
                schema: "hr",
                table: "Job",
                newName: "Vacancies");

            migrationBuilder.RenameColumn(
                name: "SectorId",
                schema: "hr",
                table: "Job",
                newName: "StatusId");

            migrationBuilder.RenameColumn(
                name: "ManagementId",
                schema: "hr",
                table: "Job",
                newName: "RequestingDepartmentId");

            migrationBuilder.RenameColumn(
                name: "JobStatusId",
                schema: "hr",
                table: "Job",
                newName: "QuotaId");

            migrationBuilder.RenameColumn(
                name: "ClosingDate",
                schema: "hr",
                table: "Job",
                newName: "Deadline");

            migrationBuilder.RenameIndex(
                name: "IX_Job_SectorId",
                schema: "hr",
                table: "Job",
                newName: "IX_Job_StatusId");

            migrationBuilder.RenameIndex(
                name: "IX_Job_ManagementId",
                schema: "hr",
                table: "Job",
                newName: "IX_Job_RequestingDepartmentId");

            migrationBuilder.RenameIndex(
                name: "IX_Job_Deleted_ClosingDate",
                schema: "hr",
                table: "Job",
                newName: "IX_Job_IsDeleted_Deadline");

            migrationBuilder.RenameIndex(
                name: "IX_Job_ClosingDate",
                schema: "hr",
                table: "Job",
                newName: "IX_Job_Deadline");

            migrationBuilder.AddColumn<int>(
                name: "Order",
                schema: "hr",
                table: "JobSkill",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Text",
                schema: "hr",
                table: "JobSkill",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Order",
                schema: "hr",
                table: "JobCondition",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Text",
                schema: "hr",
                table: "JobCondition",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<Guid>(
                name: "GenderId",
                schema: "hr",
                table: "Job",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Benefits",
                schema: "hr",
                table: "Job",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                schema: "hr",
                table: "Job",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                schema: "hr",
                table: "Job",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "JobStatus",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("0b3e5e28-0c0a-4c0c-9b6b-0af1a7f0b5c8"), "Cancelled", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "تم إلغاء الوظيفة ككل ولن يتم نشرها على المنصة (تظهر لمسؤول التوظيف فقط).", "The job has been cancelled and will not be published on the platform.", 4, false, "ملغية", "Cancelled", null, null },
                    { new Guid("6cfc1e9c-9b2b-4d20-b7b1-7a4b2c0a41d4"), "Active", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "الوظيفة جاهزة للتقديم ويمكن إرسالها للجمهور المطلوب.", "Job is open for applications and can be published to the target audience.", 2, false, "نشطة", "Active", null, null },
                    { new Guid("8c6d6c8c-9f68-471a-9f07-0c2b16d4e101"), "Draft", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "تم إنشاء الوظيفة كمسودة ولم يتم إرسالها للجمهور.", "Job is created as a draft and not visible to the public.", 1, false, "مسودة", "Draft", null, null },
                    { new Guid("b1b2c364-7b10-46b6-8b4b-9d15e2ce2a23"), "Closed", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "تم الوصول إلى تاريخ نهاية التقديم ولن يُسمح بإرسال الطلبات.", "Application end date has been reached; job is not accepting new applications.", 3, false, "متوقفة", "Closed", null, null }
                });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "Sector",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("1548322c-6c05-4754-a89c-19e9d2443d62"), "Schools", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "قطاع المدارس", "Sector for schools", 1, false, "المدارس", "Schools", null, null },
                    { new Guid("8fadf8df-ae7e-4d22-8cb8-f79ed9b14375"), "Ministry", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "قطاع الوزارة", "Sector for ministry", 2, false, "الوزارة", "Ministry", null, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Job_QuotaId",
                schema: "hr",
                table: "Job",
                column: "QuotaId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Invitations_Job_JobId",
                table: "Invitations",
                column: "JobId",
                principalSchema: "hr",
                principalTable: "Job",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Job_Department_RequestingDepartmentId",
                schema: "hr",
                table: "Job",
                column: "RequestingDepartmentId",
                principalSchema: "lkp",
                principalTable: "Department",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Job_JobQuota_QuotaId",
                schema: "hr",
                table: "Job",
                column: "QuotaId",
                principalSchema: "hr",
                principalTable: "JobQuota",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Job_JobStatus_StatusId",
                schema: "hr",
                table: "Job",
                column: "StatusId",
                principalSchema: "lkp",
                principalTable: "JobStatus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Job_Sector_WorkLocationId",
                schema: "hr",
                table: "Job",
                column: "WorkLocationId",
                principalSchema: "lkp",
                principalTable: "Sector",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
