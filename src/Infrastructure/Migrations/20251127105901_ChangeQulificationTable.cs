using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tawtheef.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeQulificationTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Qualification_QualificationLevel_LevelId",
                schema: "pro",
                table: "Qualification");

            migrationBuilder.DropColumn(
                name: "Address",
                schema: "pro",
                table: "ResidenceAddress");

            migrationBuilder.RenameColumn(
                name: "LevelId",
                schema: "pro",
                table: "Qualification",
                newName: "SubMajorId");

            migrationBuilder.RenameIndex(
                name: "IX_Qualification_LevelId",
                schema: "pro",
                table: "Qualification",
                newName: "IX_Qualification_SubMajorId");

            migrationBuilder.AlterColumn<string>(
                name: "NameEn",
                schema: "lkp",
                table: "University",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "NameAr",
                schema: "lkp",
                table: "University",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "BackendName",
                schema: "lkp",
                table: "University",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "NameEn",
                schema: "lkp",
                table: "SkillType",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "NameAr",
                schema: "lkp",
                table: "SkillType",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "BackendName",
                schema: "lkp",
                table: "SkillType",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<int>(
                name: "ZoneNo",
                schema: "pro",
                table: "ResidenceAddress",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "UnitNo",
                schema: "pro",
                table: "ResidenceAddress",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "StreetNo",
                schema: "pro",
                table: "ResidenceAddress",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "BuildingNo",
                schema: "pro",
                table: "ResidenceAddress",
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
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<Guid>(
                name: "DegreeId",
                schema: "pro",
                table: "Qualification",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<string>(
                name: "NameEn",
                schema: "lkp",
                table: "Major",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "NameAr",
                schema: "lkp",
                table: "Major",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "BackendName",
                schema: "lkp",
                table: "Major",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "NameEn",
                schema: "lkp",
                table: "Country",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "NameAr",
                schema: "lkp",
                table: "Country",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "BackendName",
                schema: "lkp",
                table: "Country",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "NameEn",
                schema: "lkp",
                table: "City",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "NameAr",
                schema: "lkp",
                table: "City",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "BackendName",
                schema: "lkp",
                table: "City",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.CreateIndex(
                name: "IX_University_BackendName",
                schema: "lkp",
                table: "University",
                column: "BackendName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_University_DisplayOrder",
                schema: "lkp",
                table: "University",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_SkillType_BackendName",
                schema: "lkp",
                table: "SkillType",
                column: "BackendName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SkillType_DisplayOrder",
                schema: "lkp",
                table: "SkillType",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_Qualification_DegreeId",
                schema: "pro",
                table: "Qualification",
                column: "DegreeId");

            migrationBuilder.CreateIndex(
                name: "IX_Major_BackendName",
                schema: "lkp",
                table: "Major",
                column: "BackendName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Major_DisplayOrder",
                schema: "lkp",
                table: "Major",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_Country_BackendName",
                schema: "lkp",
                table: "Country",
                column: "BackendName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Country_DisplayOrder",
                schema: "lkp",
                table: "Country",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_City_BackendName",
                schema: "lkp",
                table: "City",
                column: "BackendName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_City_DisplayOrder",
                schema: "lkp",
                table: "City",
                column: "DisplayOrder");

            migrationBuilder.AddForeignKey(
                name: "FK_Qualification_Major_SubMajorId",
                schema: "pro",
                table: "Qualification",
                column: "SubMajorId",
                principalSchema: "lkp",
                principalTable: "Major",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Qualification_QualificationLevel_DegreeId",
                schema: "pro",
                table: "Qualification",
                column: "DegreeId",
                principalSchema: "lkp",
                principalTable: "QualificationLevel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Qualification_Major_SubMajorId",
                schema: "pro",
                table: "Qualification");

            migrationBuilder.DropForeignKey(
                name: "FK_Qualification_QualificationLevel_DegreeId",
                schema: "pro",
                table: "Qualification");

            migrationBuilder.DropIndex(
                name: "IX_University_BackendName",
                schema: "lkp",
                table: "University");

            migrationBuilder.DropIndex(
                name: "IX_University_DisplayOrder",
                schema: "lkp",
                table: "University");

            migrationBuilder.DropIndex(
                name: "IX_SkillType_BackendName",
                schema: "lkp",
                table: "SkillType");

            migrationBuilder.DropIndex(
                name: "IX_SkillType_DisplayOrder",
                schema: "lkp",
                table: "SkillType");

            migrationBuilder.DropIndex(
                name: "IX_Qualification_DegreeId",
                schema: "pro",
                table: "Qualification");

            migrationBuilder.DropIndex(
                name: "IX_Major_BackendName",
                schema: "lkp",
                table: "Major");

            migrationBuilder.DropIndex(
                name: "IX_Major_DisplayOrder",
                schema: "lkp",
                table: "Major");

            migrationBuilder.DropIndex(
                name: "IX_Country_BackendName",
                schema: "lkp",
                table: "Country");

            migrationBuilder.DropIndex(
                name: "IX_Country_DisplayOrder",
                schema: "lkp",
                table: "Country");

            migrationBuilder.DropIndex(
                name: "IX_City_BackendName",
                schema: "lkp",
                table: "City");

            migrationBuilder.DropIndex(
                name: "IX_City_DisplayOrder",
                schema: "lkp",
                table: "City");

            migrationBuilder.DropColumn(
                name: "DegreeId",
                schema: "pro",
                table: "Qualification");

            migrationBuilder.RenameColumn(
                name: "SubMajorId",
                schema: "pro",
                table: "Qualification",
                newName: "LevelId");

            migrationBuilder.RenameIndex(
                name: "IX_Qualification_SubMajorId",
                schema: "pro",
                table: "Qualification",
                newName: "IX_Qualification_LevelId");

            migrationBuilder.AlterColumn<string>(
                name: "NameEn",
                schema: "lkp",
                table: "University",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "NameAr",
                schema: "lkp",
                table: "University",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "BackendName",
                schema: "lkp",
                table: "University",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "NameEn",
                schema: "lkp",
                table: "SkillType",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "NameAr",
                schema: "lkp",
                table: "SkillType",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "BackendName",
                schema: "lkp",
                table: "SkillType",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<int>(
                name: "ZoneNo",
                schema: "pro",
                table: "ResidenceAddress",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "UnitNo",
                schema: "pro",
                table: "ResidenceAddress",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "StreetNo",
                schema: "pro",
                table: "ResidenceAddress",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "BuildingNo",
                schema: "pro",
                table: "ResidenceAddress",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                schema: "pro",
                table: "ResidenceAddress",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "GPA",
                schema: "pro",
                table: "Qualification",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<string>(
                name: "NameEn",
                schema: "lkp",
                table: "Major",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "NameAr",
                schema: "lkp",
                table: "Major",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "BackendName",
                schema: "lkp",
                table: "Major",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "NameEn",
                schema: "lkp",
                table: "Country",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "NameAr",
                schema: "lkp",
                table: "Country",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "BackendName",
                schema: "lkp",
                table: "Country",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "NameEn",
                schema: "lkp",
                table: "City",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "NameAr",
                schema: "lkp",
                table: "City",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "BackendName",
                schema: "lkp",
                table: "City",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddForeignKey(
                name: "FK_Qualification_QualificationLevel_LevelId",
                schema: "pro",
                table: "Qualification",
                column: "LevelId",
                principalSchema: "lkp",
                principalTable: "QualificationLevel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
