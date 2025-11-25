using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tawtheef.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ResturctureProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserProfile_Resources_NationalCardIdAttachmentId",
                schema: "app",
                table: "UserProfile");

            migrationBuilder.RenameColumn(
                name: "NationalCardIdAttachmentId",
                schema: "app",
                table: "UserProfile",
                newName: "NationalCardId");

            migrationBuilder.RenameIndex(
                name: "IX_UserProfile_NationalCardIdAttachmentId",
                schema: "app",
                table: "UserProfile",
                newName: "IX_UserProfile_NationalCardId");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedDate",
                schema: "lkp",
                table: "WorkType",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 96);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedById",
                schema: "lkp",
                table: "WorkType",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 95);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "lkp",
                table: "WorkType",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .Annotation("Relational:ColumnOrder", 99);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DeletedDate",
                schema: "lkp",
                table: "WorkType",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 98);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedById",
                schema: "lkp",
                table: "WorkType",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 97);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedDate",
                schema: "lkp",
                table: "WorkType",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset")
                .Annotation("Relational:ColumnOrder", 94);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedById",
                schema: "lkp",
                table: "WorkType",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 93);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedDate",
                schema: "lkp",
                table: "UserType",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 96);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedById",
                schema: "lkp",
                table: "UserType",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 95);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "lkp",
                table: "UserType",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .Annotation("Relational:ColumnOrder", 99);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DeletedDate",
                schema: "lkp",
                table: "UserType",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 98);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedById",
                schema: "lkp",
                table: "UserType",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 97);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedDate",
                schema: "lkp",
                table: "UserType",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset")
                .Annotation("Relational:ColumnOrder", 94);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedById",
                schema: "lkp",
                table: "UserType",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 93);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedDate",
                table: "UserSession",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 96);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedById",
                table: "UserSession",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 95);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "UserSession",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .Annotation("Relational:ColumnOrder", 99);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DeletedDate",
                table: "UserSession",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 98);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedById",
                table: "UserSession",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 97);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedDate",
                table: "UserSession",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset")
                .Annotation("Relational:ColumnOrder", 94);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedById",
                table: "UserSession",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 93);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedDate",
                schema: "app",
                table: "UserProfile",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 96);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedById",
                schema: "app",
                table: "UserProfile",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 95);

            migrationBuilder.AlterColumn<string>(
                name: "NationalNumber",
                schema: "app",
                table: "UserProfile",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "app",
                table: "UserProfile",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .Annotation("Relational:ColumnOrder", 99);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DeletedDate",
                schema: "app",
                table: "UserProfile",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 98);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedById",
                schema: "app",
                table: "UserProfile",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 97);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedDate",
                schema: "app",
                table: "UserProfile",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset")
                .Annotation("Relational:ColumnOrder", 94);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedById",
                schema: "app",
                table: "UserProfile",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 93);

            migrationBuilder.AlterColumn<DateOnly>(
                name: "BirthDate",
                schema: "app",
                table: "UserProfile",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                schema: "app",
                table: "UserProfile",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedDate",
                schema: "lkp",
                table: "University",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 96);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedById",
                schema: "lkp",
                table: "University",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 95);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "lkp",
                table: "University",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .Annotation("Relational:ColumnOrder", 99);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DeletedDate",
                schema: "lkp",
                table: "University",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 98);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedById",
                schema: "lkp",
                table: "University",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 97);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedDate",
                schema: "lkp",
                table: "University",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset")
                .Annotation("Relational:ColumnOrder", 94);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedById",
                schema: "lkp",
                table: "University",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 93);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedDate",
                schema: "pro",
                table: "TrainingCourse",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 96);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedById",
                schema: "pro",
                table: "TrainingCourse",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 95);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "pro",
                table: "TrainingCourse",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .Annotation("Relational:ColumnOrder", 99);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DeletedDate",
                schema: "pro",
                table: "TrainingCourse",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 98);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedById",
                schema: "pro",
                table: "TrainingCourse",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 97);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedDate",
                schema: "pro",
                table: "TrainingCourse",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset")
                .Annotation("Relational:ColumnOrder", 94);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedById",
                schema: "pro",
                table: "TrainingCourse",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 93);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedDate",
                schema: "lkp",
                table: "TargetEntity",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 96);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedById",
                schema: "lkp",
                table: "TargetEntity",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 95);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "lkp",
                table: "TargetEntity",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .Annotation("Relational:ColumnOrder", 99);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DeletedDate",
                schema: "lkp",
                table: "TargetEntity",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 98);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedById",
                schema: "lkp",
                table: "TargetEntity",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 97);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedDate",
                schema: "lkp",
                table: "TargetEntity",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset")
                .Annotation("Relational:ColumnOrder", 94);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedById",
                schema: "lkp",
                table: "TargetEntity",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 93);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedDate",
                schema: "lkp",
                table: "StudyType",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 96);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedById",
                schema: "lkp",
                table: "StudyType",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 95);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "lkp",
                table: "StudyType",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .Annotation("Relational:ColumnOrder", 99);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DeletedDate",
                schema: "lkp",
                table: "StudyType",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 98);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedById",
                schema: "lkp",
                table: "StudyType",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 97);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedDate",
                schema: "lkp",
                table: "StudyType",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset")
                .Annotation("Relational:ColumnOrder", 94);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedById",
                schema: "lkp",
                table: "StudyType",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 93);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedDate",
                schema: "lkp",
                table: "SponsorType",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 96);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedById",
                schema: "lkp",
                table: "SponsorType",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 95);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "lkp",
                table: "SponsorType",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .Annotation("Relational:ColumnOrder", 99);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DeletedDate",
                schema: "lkp",
                table: "SponsorType",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 98);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedById",
                schema: "lkp",
                table: "SponsorType",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 97);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedDate",
                schema: "lkp",
                table: "SponsorType",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset")
                .Annotation("Relational:ColumnOrder", 94);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedById",
                schema: "lkp",
                table: "SponsorType",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 93);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedDate",
                schema: "app",
                table: "SponsorProfile",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 96);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedById",
                schema: "app",
                table: "SponsorProfile",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 95);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "app",
                table: "SponsorProfile",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .Annotation("Relational:ColumnOrder", 99);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DeletedDate",
                schema: "app",
                table: "SponsorProfile",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 98);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedById",
                schema: "app",
                table: "SponsorProfile",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 97);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedDate",
                schema: "app",
                table: "SponsorProfile",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset")
                .Annotation("Relational:ColumnOrder", 94);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedById",
                schema: "app",
                table: "SponsorProfile",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 93);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedDate",
                schema: "lkp",
                table: "SkillType",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 96);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedById",
                schema: "lkp",
                table: "SkillType",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 95);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "lkp",
                table: "SkillType",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .Annotation("Relational:ColumnOrder", 99);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DeletedDate",
                schema: "lkp",
                table: "SkillType",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 98);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedById",
                schema: "lkp",
                table: "SkillType",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 97);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedDate",
                schema: "lkp",
                table: "SkillType",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset")
                .Annotation("Relational:ColumnOrder", 94);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedById",
                schema: "lkp",
                table: "SkillType",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 93);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedDate",
                schema: "lkp",
                table: "Sector",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 96);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedById",
                schema: "lkp",
                table: "Sector",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 95);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "lkp",
                table: "Sector",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .Annotation("Relational:ColumnOrder", 99);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DeletedDate",
                schema: "lkp",
                table: "Sector",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 98);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedById",
                schema: "lkp",
                table: "Sector",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 97);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedDate",
                schema: "lkp",
                table: "Sector",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset")
                .Annotation("Relational:ColumnOrder", 94);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedById",
                schema: "lkp",
                table: "Sector",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 93);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedDate",
                table: "Resources",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 96);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedById",
                table: "Resources",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 95);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "Resources",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .Annotation("Relational:ColumnOrder", 99);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DeletedDate",
                table: "Resources",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 98);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedById",
                table: "Resources",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 97);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedDate",
                table: "Resources",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset")
                .Annotation("Relational:ColumnOrder", 94);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedById",
                table: "Resources",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 93);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedDate",
                schema: "pro",
                table: "ResidenceAddress",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 96);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedById",
                schema: "pro",
                table: "ResidenceAddress",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 95);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "pro",
                table: "ResidenceAddress",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .Annotation("Relational:ColumnOrder", 99);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DeletedDate",
                schema: "pro",
                table: "ResidenceAddress",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 98);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedById",
                schema: "pro",
                table: "ResidenceAddress",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 97);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedDate",
                schema: "pro",
                table: "ResidenceAddress",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset")
                .Annotation("Relational:ColumnOrder", 94);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedById",
                schema: "pro",
                table: "ResidenceAddress",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 93);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedDate",
                schema: "lkp",
                table: "Religion",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 96);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedById",
                schema: "lkp",
                table: "Religion",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 95);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "lkp",
                table: "Religion",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .Annotation("Relational:ColumnOrder", 99);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DeletedDate",
                schema: "lkp",
                table: "Religion",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 98);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedById",
                schema: "lkp",
                table: "Religion",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 97);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedDate",
                schema: "lkp",
                table: "Religion",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset")
                .Annotation("Relational:ColumnOrder", 94);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedById",
                schema: "lkp",
                table: "Religion",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 93);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedDate",
                table: "RefreshToken",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 96);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedById",
                table: "RefreshToken",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 95);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "RefreshToken",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .Annotation("Relational:ColumnOrder", 99);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DeletedDate",
                table: "RefreshToken",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 98);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedById",
                table: "RefreshToken",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 97);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedDate",
                table: "RefreshToken",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset")
                .Annotation("Relational:ColumnOrder", 94);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedById",
                table: "RefreshToken",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 93);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedDate",
                schema: "lkp",
                table: "RatingGrade",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 96);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedById",
                schema: "lkp",
                table: "RatingGrade",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 95);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "lkp",
                table: "RatingGrade",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .Annotation("Relational:ColumnOrder", 99);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DeletedDate",
                schema: "lkp",
                table: "RatingGrade",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 98);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedById",
                schema: "lkp",
                table: "RatingGrade",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 97);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedDate",
                schema: "lkp",
                table: "RatingGrade",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset")
                .Annotation("Relational:ColumnOrder", 94);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedById",
                schema: "lkp",
                table: "RatingGrade",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 93);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedDate",
                schema: "lkp",
                table: "QualificationLevel",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 96);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedById",
                schema: "lkp",
                table: "QualificationLevel",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 95);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "lkp",
                table: "QualificationLevel",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .Annotation("Relational:ColumnOrder", 99);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DeletedDate",
                schema: "lkp",
                table: "QualificationLevel",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 98);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedById",
                schema: "lkp",
                table: "QualificationLevel",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 97);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedDate",
                schema: "lkp",
                table: "QualificationLevel",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset")
                .Annotation("Relational:ColumnOrder", 94);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedById",
                schema: "lkp",
                table: "QualificationLevel",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 93);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedDate",
                schema: "pro",
                table: "Qualification",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 96);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedById",
                schema: "pro",
                table: "Qualification",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 95);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "pro",
                table: "Qualification",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .Annotation("Relational:ColumnOrder", 99);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DeletedDate",
                schema: "pro",
                table: "Qualification",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 98);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedById",
                schema: "pro",
                table: "Qualification",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 97);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedDate",
                schema: "pro",
                table: "Qualification",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset")
                .Annotation("Relational:ColumnOrder", 94);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedById",
                schema: "pro",
                table: "Qualification",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 93);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedDate",
                schema: "pro",
                table: "ProfileSkill",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 96);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedById",
                schema: "pro",
                table: "ProfileSkill",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 95);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "pro",
                table: "ProfileSkill",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .Annotation("Relational:ColumnOrder", 99);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DeletedDate",
                schema: "pro",
                table: "ProfileSkill",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 98);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedById",
                schema: "pro",
                table: "ProfileSkill",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 97);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedDate",
                schema: "pro",
                table: "ProfileSkill",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset")
                .Annotation("Relational:ColumnOrder", 94);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedById",
                schema: "pro",
                table: "ProfileSkill",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 93);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedDate",
                schema: "pro",
                table: "ProfileLanguage",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 96);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedById",
                schema: "pro",
                table: "ProfileLanguage",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 95);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "pro",
                table: "ProfileLanguage",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .Annotation("Relational:ColumnOrder", 99);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DeletedDate",
                schema: "pro",
                table: "ProfileLanguage",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 98);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedById",
                schema: "pro",
                table: "ProfileLanguage",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 97);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedDate",
                schema: "pro",
                table: "ProfileLanguage",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset")
                .Annotation("Relational:ColumnOrder", 94);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedById",
                schema: "pro",
                table: "ProfileLanguage",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 93);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedDate",
                schema: "pro",
                table: "ProfileAdditionalAttachment",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 96);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedById",
                schema: "pro",
                table: "ProfileAdditionalAttachment",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 95);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "pro",
                table: "ProfileAdditionalAttachment",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .Annotation("Relational:ColumnOrder", 99);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DeletedDate",
                schema: "pro",
                table: "ProfileAdditionalAttachment",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 98);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedById",
                schema: "pro",
                table: "ProfileAdditionalAttachment",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 97);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedDate",
                schema: "pro",
                table: "ProfileAdditionalAttachment",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset")
                .Annotation("Relational:ColumnOrder", 94);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedById",
                schema: "pro",
                table: "ProfileAdditionalAttachment",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 93);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedDate",
                table: "Notifications",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 96);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedById",
                table: "Notifications",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 95);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "Notifications",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .Annotation("Relational:ColumnOrder", 99);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DeletedDate",
                table: "Notifications",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 98);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedById",
                table: "Notifications",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 97);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedDate",
                table: "Notifications",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset")
                .Annotation("Relational:ColumnOrder", 94);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedById",
                table: "Notifications",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 93);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedDate",
                schema: "lkp",
                table: "MaritalStatus",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 96);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedById",
                schema: "lkp",
                table: "MaritalStatus",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 95);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "lkp",
                table: "MaritalStatus",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .Annotation("Relational:ColumnOrder", 99);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DeletedDate",
                schema: "lkp",
                table: "MaritalStatus",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 98);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedById",
                schema: "lkp",
                table: "MaritalStatus",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 97);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedDate",
                schema: "lkp",
                table: "MaritalStatus",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset")
                .Annotation("Relational:ColumnOrder", 94);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedById",
                schema: "lkp",
                table: "MaritalStatus",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 93);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedDate",
                schema: "lkp",
                table: "Major",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 96);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedById",
                schema: "lkp",
                table: "Major",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 95);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "lkp",
                table: "Major",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .Annotation("Relational:ColumnOrder", 99);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DeletedDate",
                schema: "lkp",
                table: "Major",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 98);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedById",
                schema: "lkp",
                table: "Major",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 97);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedDate",
                schema: "lkp",
                table: "Major",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset")
                .Annotation("Relational:ColumnOrder", 94);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedById",
                schema: "lkp",
                table: "Major",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 93);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedDate",
                schema: "lkp",
                table: "LanguageLevel",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 96);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedById",
                schema: "lkp",
                table: "LanguageLevel",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 95);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "lkp",
                table: "LanguageLevel",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .Annotation("Relational:ColumnOrder", 99);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DeletedDate",
                schema: "lkp",
                table: "LanguageLevel",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 98);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedById",
                schema: "lkp",
                table: "LanguageLevel",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 97);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedDate",
                schema: "lkp",
                table: "LanguageLevel",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset")
                .Annotation("Relational:ColumnOrder", 94);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedById",
                schema: "lkp",
                table: "LanguageLevel",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 93);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedDate",
                schema: "lkp",
                table: "Language",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 96);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedById",
                schema: "lkp",
                table: "Language",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 95);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "lkp",
                table: "Language",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .Annotation("Relational:ColumnOrder", 99);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DeletedDate",
                schema: "lkp",
                table: "Language",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 98);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedById",
                schema: "lkp",
                table: "Language",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 97);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedDate",
                schema: "lkp",
                table: "Language",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset")
                .Annotation("Relational:ColumnOrder", 94);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedById",
                schema: "lkp",
                table: "Language",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 93);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedDate",
                schema: "lkp",
                table: "JobStatus",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 96);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedById",
                schema: "lkp",
                table: "JobStatus",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 95);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "lkp",
                table: "JobStatus",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .Annotation("Relational:ColumnOrder", 99);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DeletedDate",
                schema: "lkp",
                table: "JobStatus",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 98);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedById",
                schema: "lkp",
                table: "JobStatus",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 97);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedDate",
                schema: "lkp",
                table: "JobStatus",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset")
                .Annotation("Relational:ColumnOrder", 94);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedById",
                schema: "lkp",
                table: "JobStatus",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 93);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedDate",
                table: "JobSkills",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 96);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedById",
                table: "JobSkills",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 95);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "JobSkills",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .Annotation("Relational:ColumnOrder", 99);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DeletedDate",
                table: "JobSkills",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 98);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedById",
                table: "JobSkills",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 97);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedDate",
                table: "JobSkills",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset")
                .Annotation("Relational:ColumnOrder", 94);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedById",
                table: "JobSkills",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 93);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedDate",
                table: "JobQuotas",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 96);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedById",
                table: "JobQuotas",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 95);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "JobQuotas",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .Annotation("Relational:ColumnOrder", 99);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DeletedDate",
                table: "JobQuotas",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 98);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedById",
                table: "JobQuotas",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 97);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedDate",
                table: "JobQuotas",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset")
                .Annotation("Relational:ColumnOrder", 94);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedById",
                table: "JobQuotas",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 93);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedDate",
                table: "JobDegrees",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 96);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedById",
                table: "JobDegrees",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 95);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "JobDegrees",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .Annotation("Relational:ColumnOrder", 99);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DeletedDate",
                table: "JobDegrees",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 98);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedById",
                table: "JobDegrees",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 97);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedDate",
                table: "JobDegrees",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset")
                .Annotation("Relational:ColumnOrder", 94);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedById",
                table: "JobDegrees",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 93);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedDate",
                table: "JobConditions",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 96);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedById",
                table: "JobConditions",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 95);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "JobConditions",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .Annotation("Relational:ColumnOrder", 99);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DeletedDate",
                table: "JobConditions",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 98);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedById",
                table: "JobConditions",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 97);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedDate",
                table: "JobConditions",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset")
                .Annotation("Relational:ColumnOrder", 94);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedById",
                table: "JobConditions",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 93);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedDate",
                schema: "lkp",
                table: "JobCategory",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 96);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedById",
                schema: "lkp",
                table: "JobCategory",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 95);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "lkp",
                table: "JobCategory",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .Annotation("Relational:ColumnOrder", 99);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DeletedDate",
                schema: "lkp",
                table: "JobCategory",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 98);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedById",
                schema: "lkp",
                table: "JobCategory",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 97);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedDate",
                schema: "lkp",
                table: "JobCategory",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset")
                .Annotation("Relational:ColumnOrder", 94);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedById",
                schema: "lkp",
                table: "JobCategory",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 93);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedDate",
                schema: "hr",
                table: "Job",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 96);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedById",
                schema: "hr",
                table: "Job",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 95);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "hr",
                table: "Job",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .Annotation("Relational:ColumnOrder", 99);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DeletedDate",
                schema: "hr",
                table: "Job",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 98);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedById",
                schema: "hr",
                table: "Job",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 97);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedDate",
                schema: "hr",
                table: "Job",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset")
                .Annotation("Relational:ColumnOrder", 94);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedById",
                schema: "hr",
                table: "Job",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 93);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedDate",
                schema: "lkp",
                table: "InvitationStatus",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 96);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedById",
                schema: "lkp",
                table: "InvitationStatus",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 95);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "lkp",
                table: "InvitationStatus",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .Annotation("Relational:ColumnOrder", 99);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DeletedDate",
                schema: "lkp",
                table: "InvitationStatus",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 98);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedById",
                schema: "lkp",
                table: "InvitationStatus",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 97);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedDate",
                schema: "lkp",
                table: "InvitationStatus",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset")
                .Annotation("Relational:ColumnOrder", 94);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedById",
                schema: "lkp",
                table: "InvitationStatus",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 93);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedDate",
                table: "Invitations",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 96);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedById",
                table: "Invitations",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 95);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "Invitations",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .Annotation("Relational:ColumnOrder", 99);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DeletedDate",
                table: "Invitations",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 98);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedById",
                table: "Invitations",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 97);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedDate",
                table: "Invitations",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset")
                .Annotation("Relational:ColumnOrder", 94);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedById",
                table: "Invitations",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 93);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedDate",
                table: "HistoryInvitation",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 96);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedById",
                table: "HistoryInvitation",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 95);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "HistoryInvitation",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .Annotation("Relational:ColumnOrder", 99);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DeletedDate",
                table: "HistoryInvitation",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 98);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedById",
                table: "HistoryInvitation",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 97);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedDate",
                table: "HistoryInvitation",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset")
                .Annotation("Relational:ColumnOrder", 94);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedById",
                table: "HistoryInvitation",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 93);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedDate",
                schema: "lkp",
                table: "Gender",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 96);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedById",
                schema: "lkp",
                table: "Gender",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 95);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "lkp",
                table: "Gender",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .Annotation("Relational:ColumnOrder", 99);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DeletedDate",
                schema: "lkp",
                table: "Gender",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 98);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedById",
                schema: "lkp",
                table: "Gender",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 97);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedDate",
                schema: "lkp",
                table: "Gender",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset")
                .Annotation("Relational:ColumnOrder", 94);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedById",
                schema: "lkp",
                table: "Gender",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 93);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedDate",
                schema: "pro",
                table: "Experience",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 96);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedById",
                schema: "pro",
                table: "Experience",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 95);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "pro",
                table: "Experience",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .Annotation("Relational:ColumnOrder", 99);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DeletedDate",
                schema: "pro",
                table: "Experience",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 98);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedById",
                schema: "pro",
                table: "Experience",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 97);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedDate",
                schema: "pro",
                table: "Experience",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset")
                .Annotation("Relational:ColumnOrder", 94);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedById",
                schema: "pro",
                table: "Experience",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 93);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedDate",
                table: "EntityLog",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 96);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedById",
                table: "EntityLog",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 95);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "EntityLog",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .Annotation("Relational:ColumnOrder", 99);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DeletedDate",
                table: "EntityLog",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 98);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedById",
                table: "EntityLog",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 97);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedDate",
                table: "EntityLog",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset")
                .Annotation("Relational:ColumnOrder", 94);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedById",
                table: "EntityLog",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 93);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedDate",
                table: "EmailTemplates",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 96);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedById",
                table: "EmailTemplates",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 95);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "EmailTemplates",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .Annotation("Relational:ColumnOrder", 99);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DeletedDate",
                table: "EmailTemplates",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 98);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedById",
                table: "EmailTemplates",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 97);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedDate",
                table: "EmailTemplates",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset")
                .Annotation("Relational:ColumnOrder", 94);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedById",
                table: "EmailTemplates",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 93);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedDate",
                table: "EmailQueues",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 96);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedById",
                table: "EmailQueues",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 95);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "EmailQueues",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .Annotation("Relational:ColumnOrder", 99);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DeletedDate",
                table: "EmailQueues",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 98);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedById",
                table: "EmailQueues",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 97);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedDate",
                table: "EmailQueues",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset")
                .Annotation("Relational:ColumnOrder", 94);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedById",
                table: "EmailQueues",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 93);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedDate",
                schema: "lkp",
                table: "Department",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 96);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedById",
                schema: "lkp",
                table: "Department",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 95);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "lkp",
                table: "Department",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .Annotation("Relational:ColumnOrder", 99);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DeletedDate",
                schema: "lkp",
                table: "Department",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 98);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedById",
                schema: "lkp",
                table: "Department",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 97);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedDate",
                schema: "lkp",
                table: "Department",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset")
                .Annotation("Relational:ColumnOrder", 94);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedById",
                schema: "lkp",
                table: "Department",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 93);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedDate",
                schema: "lkp",
                table: "Degree",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 96);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedById",
                schema: "lkp",
                table: "Degree",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 95);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "lkp",
                table: "Degree",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .Annotation("Relational:ColumnOrder", 99);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DeletedDate",
                schema: "lkp",
                table: "Degree",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 98);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedById",
                schema: "lkp",
                table: "Degree",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 97);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedDate",
                schema: "lkp",
                table: "Degree",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset")
                .Annotation("Relational:ColumnOrder", 94);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedById",
                schema: "lkp",
                table: "Degree",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 93);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedDate",
                schema: "lkp",
                table: "Country",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 96);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedById",
                schema: "lkp",
                table: "Country",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 95);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "lkp",
                table: "Country",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .Annotation("Relational:ColumnOrder", 99);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DeletedDate",
                schema: "lkp",
                table: "Country",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 98);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedById",
                schema: "lkp",
                table: "Country",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 97);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedDate",
                schema: "lkp",
                table: "Country",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset")
                .Annotation("Relational:ColumnOrder", 94);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedById",
                schema: "lkp",
                table: "Country",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 93);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedDate",
                table: "ContactVerification",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 96);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedById",
                table: "ContactVerification",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 95);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "ContactVerification",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .Annotation("Relational:ColumnOrder", 99);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DeletedDate",
                table: "ContactVerification",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 98);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedById",
                table: "ContactVerification",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 97);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedDate",
                table: "ContactVerification",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset")
                .Annotation("Relational:ColumnOrder", 94);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedById",
                table: "ContactVerification",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 93);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedDate",
                schema: "lkp",
                table: "City",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 96);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedById",
                schema: "lkp",
                table: "City",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 95);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "lkp",
                table: "City",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .Annotation("Relational:ColumnOrder", 99);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DeletedDate",
                schema: "lkp",
                table: "City",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 98);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedById",
                schema: "lkp",
                table: "City",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 97);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedDate",
                schema: "lkp",
                table: "City",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset")
                .Annotation("Relational:ColumnOrder", 94);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedById",
                schema: "lkp",
                table: "City",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 93);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedDate",
                schema: "lkp",
                table: "CandidateType",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 96);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedById",
                schema: "lkp",
                table: "CandidateType",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 95);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "lkp",
                table: "CandidateType",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .Annotation("Relational:ColumnOrder", 99);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DeletedDate",
                schema: "lkp",
                table: "CandidateType",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 98);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedById",
                schema: "lkp",
                table: "CandidateType",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 97);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedDate",
                schema: "lkp",
                table: "CandidateType",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset")
                .Annotation("Relational:ColumnOrder", 94);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedById",
                schema: "lkp",
                table: "CandidateType",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 93);

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
                    Organization = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: true),
                    CertificateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
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
                name: "FK_UserProfile_Resources_NationalCardId",
                schema: "app",
                table: "UserProfile",
                column: "NationalCardId",
                principalTable: "Resources",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserProfile_Resources_NationalCardId",
                schema: "app",
                table: "UserProfile");

            migrationBuilder.DropTable(
                name: "Achievement",
                schema: "pro");

            migrationBuilder.DropColumn(
                name: "Address",
                schema: "app",
                table: "UserProfile");

            migrationBuilder.RenameColumn(
                name: "NationalCardId",
                schema: "app",
                table: "UserProfile",
                newName: "NationalCardIdAttachmentId");

            migrationBuilder.RenameIndex(
                name: "IX_UserProfile_NationalCardId",
                schema: "app",
                table: "UserProfile",
                newName: "IX_UserProfile_NationalCardIdAttachmentId");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedDate",
                schema: "lkp",
                table: "WorkType",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 96);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedById",
                schema: "lkp",
                table: "WorkType",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 95);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "lkp",
                table: "WorkType",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .OldAnnotation("Relational:ColumnOrder", 99);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DeletedDate",
                schema: "lkp",
                table: "WorkType",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 98);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedById",
                schema: "lkp",
                table: "WorkType",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 97);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedDate",
                schema: "lkp",
                table: "WorkType",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset")
                .OldAnnotation("Relational:ColumnOrder", 94);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedById",
                schema: "lkp",
                table: "WorkType",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 93);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedDate",
                schema: "lkp",
                table: "UserType",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 96);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedById",
                schema: "lkp",
                table: "UserType",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 95);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "lkp",
                table: "UserType",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .OldAnnotation("Relational:ColumnOrder", 99);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DeletedDate",
                schema: "lkp",
                table: "UserType",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 98);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedById",
                schema: "lkp",
                table: "UserType",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 97);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedDate",
                schema: "lkp",
                table: "UserType",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset")
                .OldAnnotation("Relational:ColumnOrder", 94);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedById",
                schema: "lkp",
                table: "UserType",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 93);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedDate",
                table: "UserSession",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 96);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedById",
                table: "UserSession",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 95);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "UserSession",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .OldAnnotation("Relational:ColumnOrder", 99);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DeletedDate",
                table: "UserSession",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 98);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedById",
                table: "UserSession",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 97);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedDate",
                table: "UserSession",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset")
                .OldAnnotation("Relational:ColumnOrder", 94);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedById",
                table: "UserSession",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 93);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedDate",
                schema: "app",
                table: "UserProfile",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 96);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedById",
                schema: "app",
                table: "UserProfile",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 95);

            migrationBuilder.AlterColumn<int>(
                name: "NationalNumber",
                schema: "app",
                table: "UserProfile",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "app",
                table: "UserProfile",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .OldAnnotation("Relational:ColumnOrder", 99);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DeletedDate",
                schema: "app",
                table: "UserProfile",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 98);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedById",
                schema: "app",
                table: "UserProfile",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 97);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedDate",
                schema: "app",
                table: "UserProfile",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset")
                .OldAnnotation("Relational:ColumnOrder", 94);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedById",
                schema: "app",
                table: "UserProfile",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 93);

            migrationBuilder.AlterColumn<DateOnly>(
                name: "BirthDate",
                schema: "app",
                table: "UserProfile",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1),
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedDate",
                schema: "lkp",
                table: "University",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 96);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedById",
                schema: "lkp",
                table: "University",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 95);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "lkp",
                table: "University",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .OldAnnotation("Relational:ColumnOrder", 99);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DeletedDate",
                schema: "lkp",
                table: "University",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 98);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedById",
                schema: "lkp",
                table: "University",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 97);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedDate",
                schema: "lkp",
                table: "University",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset")
                .OldAnnotation("Relational:ColumnOrder", 94);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedById",
                schema: "lkp",
                table: "University",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 93);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedDate",
                schema: "pro",
                table: "TrainingCourse",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 96);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedById",
                schema: "pro",
                table: "TrainingCourse",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 95);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "pro",
                table: "TrainingCourse",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .OldAnnotation("Relational:ColumnOrder", 99);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DeletedDate",
                schema: "pro",
                table: "TrainingCourse",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 98);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedById",
                schema: "pro",
                table: "TrainingCourse",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 97);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedDate",
                schema: "pro",
                table: "TrainingCourse",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset")
                .OldAnnotation("Relational:ColumnOrder", 94);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedById",
                schema: "pro",
                table: "TrainingCourse",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 93);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedDate",
                schema: "lkp",
                table: "TargetEntity",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 96);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedById",
                schema: "lkp",
                table: "TargetEntity",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 95);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "lkp",
                table: "TargetEntity",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .OldAnnotation("Relational:ColumnOrder", 99);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DeletedDate",
                schema: "lkp",
                table: "TargetEntity",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 98);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedById",
                schema: "lkp",
                table: "TargetEntity",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 97);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedDate",
                schema: "lkp",
                table: "TargetEntity",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset")
                .OldAnnotation("Relational:ColumnOrder", 94);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedById",
                schema: "lkp",
                table: "TargetEntity",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 93);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedDate",
                schema: "lkp",
                table: "StudyType",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 96);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedById",
                schema: "lkp",
                table: "StudyType",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 95);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "lkp",
                table: "StudyType",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .OldAnnotation("Relational:ColumnOrder", 99);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DeletedDate",
                schema: "lkp",
                table: "StudyType",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 98);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedById",
                schema: "lkp",
                table: "StudyType",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 97);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedDate",
                schema: "lkp",
                table: "StudyType",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset")
                .OldAnnotation("Relational:ColumnOrder", 94);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedById",
                schema: "lkp",
                table: "StudyType",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 93);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedDate",
                schema: "lkp",
                table: "SponsorType",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 96);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedById",
                schema: "lkp",
                table: "SponsorType",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 95);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "lkp",
                table: "SponsorType",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .OldAnnotation("Relational:ColumnOrder", 99);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DeletedDate",
                schema: "lkp",
                table: "SponsorType",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 98);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedById",
                schema: "lkp",
                table: "SponsorType",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 97);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedDate",
                schema: "lkp",
                table: "SponsorType",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset")
                .OldAnnotation("Relational:ColumnOrder", 94);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedById",
                schema: "lkp",
                table: "SponsorType",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 93);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedDate",
                schema: "app",
                table: "SponsorProfile",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 96);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedById",
                schema: "app",
                table: "SponsorProfile",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 95);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "app",
                table: "SponsorProfile",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .OldAnnotation("Relational:ColumnOrder", 99);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DeletedDate",
                schema: "app",
                table: "SponsorProfile",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 98);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedById",
                schema: "app",
                table: "SponsorProfile",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 97);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedDate",
                schema: "app",
                table: "SponsorProfile",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset")
                .OldAnnotation("Relational:ColumnOrder", 94);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedById",
                schema: "app",
                table: "SponsorProfile",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 93);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedDate",
                schema: "lkp",
                table: "SkillType",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 96);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedById",
                schema: "lkp",
                table: "SkillType",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 95);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "lkp",
                table: "SkillType",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .OldAnnotation("Relational:ColumnOrder", 99);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DeletedDate",
                schema: "lkp",
                table: "SkillType",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 98);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedById",
                schema: "lkp",
                table: "SkillType",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 97);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedDate",
                schema: "lkp",
                table: "SkillType",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset")
                .OldAnnotation("Relational:ColumnOrder", 94);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedById",
                schema: "lkp",
                table: "SkillType",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 93);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedDate",
                schema: "lkp",
                table: "Sector",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 96);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedById",
                schema: "lkp",
                table: "Sector",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 95);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "lkp",
                table: "Sector",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .OldAnnotation("Relational:ColumnOrder", 99);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DeletedDate",
                schema: "lkp",
                table: "Sector",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 98);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedById",
                schema: "lkp",
                table: "Sector",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 97);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedDate",
                schema: "lkp",
                table: "Sector",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset")
                .OldAnnotation("Relational:ColumnOrder", 94);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedById",
                schema: "lkp",
                table: "Sector",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 93);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedDate",
                table: "Resources",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 96);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedById",
                table: "Resources",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 95);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "Resources",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .OldAnnotation("Relational:ColumnOrder", 99);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DeletedDate",
                table: "Resources",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 98);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedById",
                table: "Resources",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 97);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedDate",
                table: "Resources",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset")
                .OldAnnotation("Relational:ColumnOrder", 94);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedById",
                table: "Resources",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 93);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedDate",
                schema: "pro",
                table: "ResidenceAddress",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 96);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedById",
                schema: "pro",
                table: "ResidenceAddress",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 95);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "pro",
                table: "ResidenceAddress",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .OldAnnotation("Relational:ColumnOrder", 99);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DeletedDate",
                schema: "pro",
                table: "ResidenceAddress",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 98);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedById",
                schema: "pro",
                table: "ResidenceAddress",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 97);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedDate",
                schema: "pro",
                table: "ResidenceAddress",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset")
                .OldAnnotation("Relational:ColumnOrder", 94);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedById",
                schema: "pro",
                table: "ResidenceAddress",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 93);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedDate",
                schema: "lkp",
                table: "Religion",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 96);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedById",
                schema: "lkp",
                table: "Religion",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 95);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "lkp",
                table: "Religion",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .OldAnnotation("Relational:ColumnOrder", 99);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DeletedDate",
                schema: "lkp",
                table: "Religion",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 98);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedById",
                schema: "lkp",
                table: "Religion",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 97);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedDate",
                schema: "lkp",
                table: "Religion",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset")
                .OldAnnotation("Relational:ColumnOrder", 94);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedById",
                schema: "lkp",
                table: "Religion",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 93);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedDate",
                table: "RefreshToken",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 96);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedById",
                table: "RefreshToken",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 95);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "RefreshToken",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .OldAnnotation("Relational:ColumnOrder", 99);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DeletedDate",
                table: "RefreshToken",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 98);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedById",
                table: "RefreshToken",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 97);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedDate",
                table: "RefreshToken",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset")
                .OldAnnotation("Relational:ColumnOrder", 94);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedById",
                table: "RefreshToken",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 93);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedDate",
                schema: "lkp",
                table: "RatingGrade",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 96);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedById",
                schema: "lkp",
                table: "RatingGrade",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 95);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "lkp",
                table: "RatingGrade",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .OldAnnotation("Relational:ColumnOrder", 99);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DeletedDate",
                schema: "lkp",
                table: "RatingGrade",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 98);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedById",
                schema: "lkp",
                table: "RatingGrade",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 97);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedDate",
                schema: "lkp",
                table: "RatingGrade",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset")
                .OldAnnotation("Relational:ColumnOrder", 94);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedById",
                schema: "lkp",
                table: "RatingGrade",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 93);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedDate",
                schema: "lkp",
                table: "QualificationLevel",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 96);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedById",
                schema: "lkp",
                table: "QualificationLevel",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 95);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "lkp",
                table: "QualificationLevel",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .OldAnnotation("Relational:ColumnOrder", 99);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DeletedDate",
                schema: "lkp",
                table: "QualificationLevel",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 98);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedById",
                schema: "lkp",
                table: "QualificationLevel",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 97);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedDate",
                schema: "lkp",
                table: "QualificationLevel",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset")
                .OldAnnotation("Relational:ColumnOrder", 94);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedById",
                schema: "lkp",
                table: "QualificationLevel",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 93);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedDate",
                schema: "pro",
                table: "Qualification",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 96);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedById",
                schema: "pro",
                table: "Qualification",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 95);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "pro",
                table: "Qualification",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .OldAnnotation("Relational:ColumnOrder", 99);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DeletedDate",
                schema: "pro",
                table: "Qualification",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 98);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedById",
                schema: "pro",
                table: "Qualification",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 97);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedDate",
                schema: "pro",
                table: "Qualification",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset")
                .OldAnnotation("Relational:ColumnOrder", 94);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedById",
                schema: "pro",
                table: "Qualification",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 93);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedDate",
                schema: "pro",
                table: "ProfileSkill",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 96);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedById",
                schema: "pro",
                table: "ProfileSkill",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 95);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "pro",
                table: "ProfileSkill",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .OldAnnotation("Relational:ColumnOrder", 99);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DeletedDate",
                schema: "pro",
                table: "ProfileSkill",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 98);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedById",
                schema: "pro",
                table: "ProfileSkill",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 97);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedDate",
                schema: "pro",
                table: "ProfileSkill",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset")
                .OldAnnotation("Relational:ColumnOrder", 94);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedById",
                schema: "pro",
                table: "ProfileSkill",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 93);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedDate",
                schema: "pro",
                table: "ProfileLanguage",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 96);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedById",
                schema: "pro",
                table: "ProfileLanguage",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 95);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "pro",
                table: "ProfileLanguage",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .OldAnnotation("Relational:ColumnOrder", 99);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DeletedDate",
                schema: "pro",
                table: "ProfileLanguage",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 98);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedById",
                schema: "pro",
                table: "ProfileLanguage",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 97);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedDate",
                schema: "pro",
                table: "ProfileLanguage",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset")
                .OldAnnotation("Relational:ColumnOrder", 94);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedById",
                schema: "pro",
                table: "ProfileLanguage",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 93);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedDate",
                schema: "pro",
                table: "ProfileAdditionalAttachment",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 96);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedById",
                schema: "pro",
                table: "ProfileAdditionalAttachment",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 95);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "pro",
                table: "ProfileAdditionalAttachment",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .OldAnnotation("Relational:ColumnOrder", 99);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DeletedDate",
                schema: "pro",
                table: "ProfileAdditionalAttachment",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 98);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedById",
                schema: "pro",
                table: "ProfileAdditionalAttachment",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 97);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedDate",
                schema: "pro",
                table: "ProfileAdditionalAttachment",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset")
                .OldAnnotation("Relational:ColumnOrder", 94);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedById",
                schema: "pro",
                table: "ProfileAdditionalAttachment",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 93);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedDate",
                table: "Notifications",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 96);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedById",
                table: "Notifications",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 95);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "Notifications",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .OldAnnotation("Relational:ColumnOrder", 99);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DeletedDate",
                table: "Notifications",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 98);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedById",
                table: "Notifications",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 97);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedDate",
                table: "Notifications",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset")
                .OldAnnotation("Relational:ColumnOrder", 94);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedById",
                table: "Notifications",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 93);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedDate",
                schema: "lkp",
                table: "MaritalStatus",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 96);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedById",
                schema: "lkp",
                table: "MaritalStatus",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 95);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "lkp",
                table: "MaritalStatus",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .OldAnnotation("Relational:ColumnOrder", 99);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DeletedDate",
                schema: "lkp",
                table: "MaritalStatus",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 98);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedById",
                schema: "lkp",
                table: "MaritalStatus",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 97);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedDate",
                schema: "lkp",
                table: "MaritalStatus",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset")
                .OldAnnotation("Relational:ColumnOrder", 94);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedById",
                schema: "lkp",
                table: "MaritalStatus",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 93);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedDate",
                schema: "lkp",
                table: "Major",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 96);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedById",
                schema: "lkp",
                table: "Major",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 95);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "lkp",
                table: "Major",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .OldAnnotation("Relational:ColumnOrder", 99);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DeletedDate",
                schema: "lkp",
                table: "Major",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 98);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedById",
                schema: "lkp",
                table: "Major",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 97);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedDate",
                schema: "lkp",
                table: "Major",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset")
                .OldAnnotation("Relational:ColumnOrder", 94);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedById",
                schema: "lkp",
                table: "Major",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 93);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedDate",
                schema: "lkp",
                table: "LanguageLevel",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 96);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedById",
                schema: "lkp",
                table: "LanguageLevel",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 95);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "lkp",
                table: "LanguageLevel",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .OldAnnotation("Relational:ColumnOrder", 99);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DeletedDate",
                schema: "lkp",
                table: "LanguageLevel",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 98);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedById",
                schema: "lkp",
                table: "LanguageLevel",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 97);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedDate",
                schema: "lkp",
                table: "LanguageLevel",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset")
                .OldAnnotation("Relational:ColumnOrder", 94);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedById",
                schema: "lkp",
                table: "LanguageLevel",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 93);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedDate",
                schema: "lkp",
                table: "Language",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 96);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedById",
                schema: "lkp",
                table: "Language",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 95);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "lkp",
                table: "Language",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .OldAnnotation("Relational:ColumnOrder", 99);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DeletedDate",
                schema: "lkp",
                table: "Language",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 98);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedById",
                schema: "lkp",
                table: "Language",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 97);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedDate",
                schema: "lkp",
                table: "Language",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset")
                .OldAnnotation("Relational:ColumnOrder", 94);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedById",
                schema: "lkp",
                table: "Language",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 93);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedDate",
                schema: "lkp",
                table: "JobStatus",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 96);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedById",
                schema: "lkp",
                table: "JobStatus",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 95);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "lkp",
                table: "JobStatus",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .OldAnnotation("Relational:ColumnOrder", 99);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DeletedDate",
                schema: "lkp",
                table: "JobStatus",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 98);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedById",
                schema: "lkp",
                table: "JobStatus",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 97);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedDate",
                schema: "lkp",
                table: "JobStatus",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset")
                .OldAnnotation("Relational:ColumnOrder", 94);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedById",
                schema: "lkp",
                table: "JobStatus",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 93);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedDate",
                table: "JobSkills",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 96);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedById",
                table: "JobSkills",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 95);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "JobSkills",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .OldAnnotation("Relational:ColumnOrder", 99);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DeletedDate",
                table: "JobSkills",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 98);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedById",
                table: "JobSkills",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 97);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedDate",
                table: "JobSkills",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset")
                .OldAnnotation("Relational:ColumnOrder", 94);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedById",
                table: "JobSkills",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 93);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedDate",
                table: "JobQuotas",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 96);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedById",
                table: "JobQuotas",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 95);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "JobQuotas",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .OldAnnotation("Relational:ColumnOrder", 99);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DeletedDate",
                table: "JobQuotas",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 98);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedById",
                table: "JobQuotas",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 97);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedDate",
                table: "JobQuotas",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset")
                .OldAnnotation("Relational:ColumnOrder", 94);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedById",
                table: "JobQuotas",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 93);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedDate",
                table: "JobDegrees",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 96);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedById",
                table: "JobDegrees",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 95);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "JobDegrees",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .OldAnnotation("Relational:ColumnOrder", 99);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DeletedDate",
                table: "JobDegrees",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 98);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedById",
                table: "JobDegrees",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 97);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedDate",
                table: "JobDegrees",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset")
                .OldAnnotation("Relational:ColumnOrder", 94);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedById",
                table: "JobDegrees",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 93);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedDate",
                table: "JobConditions",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 96);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedById",
                table: "JobConditions",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 95);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "JobConditions",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .OldAnnotation("Relational:ColumnOrder", 99);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DeletedDate",
                table: "JobConditions",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 98);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedById",
                table: "JobConditions",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 97);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedDate",
                table: "JobConditions",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset")
                .OldAnnotation("Relational:ColumnOrder", 94);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedById",
                table: "JobConditions",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 93);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedDate",
                schema: "lkp",
                table: "JobCategory",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 96);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedById",
                schema: "lkp",
                table: "JobCategory",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 95);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "lkp",
                table: "JobCategory",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .OldAnnotation("Relational:ColumnOrder", 99);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DeletedDate",
                schema: "lkp",
                table: "JobCategory",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 98);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedById",
                schema: "lkp",
                table: "JobCategory",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 97);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedDate",
                schema: "lkp",
                table: "JobCategory",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset")
                .OldAnnotation("Relational:ColumnOrder", 94);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedById",
                schema: "lkp",
                table: "JobCategory",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 93);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedDate",
                schema: "hr",
                table: "Job",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 96);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedById",
                schema: "hr",
                table: "Job",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 95);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "hr",
                table: "Job",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .OldAnnotation("Relational:ColumnOrder", 99);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DeletedDate",
                schema: "hr",
                table: "Job",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 98);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedById",
                schema: "hr",
                table: "Job",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 97);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedDate",
                schema: "hr",
                table: "Job",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset")
                .OldAnnotation("Relational:ColumnOrder", 94);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedById",
                schema: "hr",
                table: "Job",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 93);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedDate",
                schema: "lkp",
                table: "InvitationStatus",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 96);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedById",
                schema: "lkp",
                table: "InvitationStatus",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 95);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "lkp",
                table: "InvitationStatus",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .OldAnnotation("Relational:ColumnOrder", 99);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DeletedDate",
                schema: "lkp",
                table: "InvitationStatus",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 98);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedById",
                schema: "lkp",
                table: "InvitationStatus",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 97);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedDate",
                schema: "lkp",
                table: "InvitationStatus",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset")
                .OldAnnotation("Relational:ColumnOrder", 94);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedById",
                schema: "lkp",
                table: "InvitationStatus",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 93);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedDate",
                table: "Invitations",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 96);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedById",
                table: "Invitations",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 95);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "Invitations",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .OldAnnotation("Relational:ColumnOrder", 99);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DeletedDate",
                table: "Invitations",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 98);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedById",
                table: "Invitations",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 97);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedDate",
                table: "Invitations",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset")
                .OldAnnotation("Relational:ColumnOrder", 94);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedById",
                table: "Invitations",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 93);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedDate",
                table: "HistoryInvitation",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 96);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedById",
                table: "HistoryInvitation",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 95);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "HistoryInvitation",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .OldAnnotation("Relational:ColumnOrder", 99);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DeletedDate",
                table: "HistoryInvitation",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 98);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedById",
                table: "HistoryInvitation",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 97);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedDate",
                table: "HistoryInvitation",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset")
                .OldAnnotation("Relational:ColumnOrder", 94);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedById",
                table: "HistoryInvitation",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 93);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedDate",
                schema: "lkp",
                table: "Gender",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 96);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedById",
                schema: "lkp",
                table: "Gender",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 95);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "lkp",
                table: "Gender",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .OldAnnotation("Relational:ColumnOrder", 99);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DeletedDate",
                schema: "lkp",
                table: "Gender",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 98);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedById",
                schema: "lkp",
                table: "Gender",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 97);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedDate",
                schema: "lkp",
                table: "Gender",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset")
                .OldAnnotation("Relational:ColumnOrder", 94);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedById",
                schema: "lkp",
                table: "Gender",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 93);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedDate",
                schema: "pro",
                table: "Experience",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 96);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedById",
                schema: "pro",
                table: "Experience",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 95);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "pro",
                table: "Experience",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .OldAnnotation("Relational:ColumnOrder", 99);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DeletedDate",
                schema: "pro",
                table: "Experience",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 98);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedById",
                schema: "pro",
                table: "Experience",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 97);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedDate",
                schema: "pro",
                table: "Experience",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset")
                .OldAnnotation("Relational:ColumnOrder", 94);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedById",
                schema: "pro",
                table: "Experience",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 93);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedDate",
                table: "EntityLog",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 96);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedById",
                table: "EntityLog",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 95);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "EntityLog",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .OldAnnotation("Relational:ColumnOrder", 99);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DeletedDate",
                table: "EntityLog",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 98);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedById",
                table: "EntityLog",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 97);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedDate",
                table: "EntityLog",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset")
                .OldAnnotation("Relational:ColumnOrder", 94);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedById",
                table: "EntityLog",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 93);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedDate",
                table: "EmailTemplates",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 96);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedById",
                table: "EmailTemplates",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 95);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "EmailTemplates",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .OldAnnotation("Relational:ColumnOrder", 99);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DeletedDate",
                table: "EmailTemplates",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 98);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedById",
                table: "EmailTemplates",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 97);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedDate",
                table: "EmailTemplates",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset")
                .OldAnnotation("Relational:ColumnOrder", 94);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedById",
                table: "EmailTemplates",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 93);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedDate",
                table: "EmailQueues",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 96);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedById",
                table: "EmailQueues",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 95);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "EmailQueues",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .OldAnnotation("Relational:ColumnOrder", 99);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DeletedDate",
                table: "EmailQueues",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 98);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedById",
                table: "EmailQueues",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 97);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedDate",
                table: "EmailQueues",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset")
                .OldAnnotation("Relational:ColumnOrder", 94);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedById",
                table: "EmailQueues",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 93);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedDate",
                schema: "lkp",
                table: "Department",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 96);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedById",
                schema: "lkp",
                table: "Department",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 95);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "lkp",
                table: "Department",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .OldAnnotation("Relational:ColumnOrder", 99);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DeletedDate",
                schema: "lkp",
                table: "Department",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 98);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedById",
                schema: "lkp",
                table: "Department",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 97);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedDate",
                schema: "lkp",
                table: "Department",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset")
                .OldAnnotation("Relational:ColumnOrder", 94);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedById",
                schema: "lkp",
                table: "Department",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 93);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedDate",
                schema: "lkp",
                table: "Degree",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 96);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedById",
                schema: "lkp",
                table: "Degree",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 95);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "lkp",
                table: "Degree",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .OldAnnotation("Relational:ColumnOrder", 99);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DeletedDate",
                schema: "lkp",
                table: "Degree",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 98);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedById",
                schema: "lkp",
                table: "Degree",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 97);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedDate",
                schema: "lkp",
                table: "Degree",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset")
                .OldAnnotation("Relational:ColumnOrder", 94);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedById",
                schema: "lkp",
                table: "Degree",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 93);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedDate",
                schema: "lkp",
                table: "Country",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 96);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedById",
                schema: "lkp",
                table: "Country",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 95);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "lkp",
                table: "Country",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .OldAnnotation("Relational:ColumnOrder", 99);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DeletedDate",
                schema: "lkp",
                table: "Country",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 98);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedById",
                schema: "lkp",
                table: "Country",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 97);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedDate",
                schema: "lkp",
                table: "Country",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset")
                .OldAnnotation("Relational:ColumnOrder", 94);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedById",
                schema: "lkp",
                table: "Country",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 93);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedDate",
                table: "ContactVerification",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 96);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedById",
                table: "ContactVerification",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 95);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "ContactVerification",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .OldAnnotation("Relational:ColumnOrder", 99);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DeletedDate",
                table: "ContactVerification",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 98);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedById",
                table: "ContactVerification",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 97);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedDate",
                table: "ContactVerification",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset")
                .OldAnnotation("Relational:ColumnOrder", 94);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedById",
                table: "ContactVerification",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 93);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedDate",
                schema: "lkp",
                table: "City",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 96);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedById",
                schema: "lkp",
                table: "City",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 95);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "lkp",
                table: "City",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .OldAnnotation("Relational:ColumnOrder", 99);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DeletedDate",
                schema: "lkp",
                table: "City",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 98);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedById",
                schema: "lkp",
                table: "City",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 97);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedDate",
                schema: "lkp",
                table: "City",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset")
                .OldAnnotation("Relational:ColumnOrder", 94);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedById",
                schema: "lkp",
                table: "City",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 93);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedDate",
                schema: "lkp",
                table: "CandidateType",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 96);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedById",
                schema: "lkp",
                table: "CandidateType",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 95);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "lkp",
                table: "CandidateType",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .OldAnnotation("Relational:ColumnOrder", 99);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DeletedDate",
                schema: "lkp",
                table: "CandidateType",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 98);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedById",
                schema: "lkp",
                table: "CandidateType",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 97);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedDate",
                schema: "lkp",
                table: "CandidateType",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset")
                .OldAnnotation("Relational:ColumnOrder", 94);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedById",
                schema: "lkp",
                table: "CandidateType",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 93);

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfile_Resources_NationalCardIdAttachmentId",
                schema: "app",
                table: "UserProfile",
                column: "NationalCardIdAttachmentId",
                principalTable: "Resources",
                principalColumn: "Id");
        }
    }
}
