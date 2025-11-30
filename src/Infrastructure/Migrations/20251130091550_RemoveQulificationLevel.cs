using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Tawtheef.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveQulificationLevel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Qualification_QualificationLevel_DegreeId",
                schema: "pro",
                table: "Qualification");

            migrationBuilder.DropTable(
                name: "QualificationLevel",
                schema: "lkp");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Qualification_Degree_DegreeId",
                schema: "pro",
                table: "Qualification",
                column: "DegreeId",
                principalSchema: "lkp",
                principalTable: "Degree",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Qualification_Degree_DegreeId",
                schema: "pro",
                table: "Qualification");

            migrationBuilder.AlterColumn<int>(
                name: "GraduationYear",
                schema: "pro",
                table: "Qualification",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateTable(
                name: "QualificationLevel",
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
                    DescriptionAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DescriptionEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QualificationLevel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QualificationLevel_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QualificationLevel_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QualificationLevel_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "QualificationLevel",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("39ffab2f-86d1-4db3-9210-113a4fcd77e8"), "Bachelor", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 3, false, "بكالوريوس", "Bachelor", null, null },
                    { new Guid("7137f1ec-1c46-414a-9071-dc40e984cc16"), "Master", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 4, false, "ماجستير", "Master", null, null },
                    { new Guid("78292166-dff6-416f-ab04-0547331cbfee"), "HighSchool", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 1, false, "الثانوية العامة", "High School", null, null },
                    { new Guid("a99f5bdc-a1a2-46ca-9211-0c85bddd1f75"), "Doctorate", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 5, false, "دكتوراه", "Doctorate", null, null },
                    { new Guid("d3a7fa85-6e1a-489d-9606-1ab786d46066"), "Diploma", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 2, false, "دبلوم", "Diploma", null, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_QualificationLevel_BackendName",
                schema: "lkp",
                table: "QualificationLevel",
                column: "BackendName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_QualificationLevel_CreatedById",
                schema: "lkp",
                table: "QualificationLevel",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_QualificationLevel_DeletedById",
                schema: "lkp",
                table: "QualificationLevel",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_QualificationLevel_DisplayOrder",
                schema: "lkp",
                table: "QualificationLevel",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_QualificationLevel_UpdatedById",
                schema: "lkp",
                table: "QualificationLevel",
                column: "UpdatedById");

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
    }
}
