using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Tawtheef.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Phase2MissingMigrations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -2029722188);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -1603357864);

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "TestSlotStatus",
                keyColumn: "Id",
                keyValue: new Guid("02a5a982-e326-403c-a8da-34b2f33d88f0"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "TestSlotStatus",
                keyColumn: "Id",
                keyValue: new Guid("4ff15b36-d258-48f4-ab1c-2380dfc0d1b0"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "TestSlotStatus",
                keyColumn: "Id",
                keyValue: new Guid("7995eaf8-baaf-4dcc-a30d-0becc5a8e4d8"));

           

            migrationBuilder.AlterColumn<string>(
                name: "DepartmentNumber",
                schema: "lkp",
                table: "Management",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
            
            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Management",
                keyColumn: "Id",
                keyValue: new Guid("453aa49d-8f16-a85b-9991-c8355ac8bf00"),
                column: "DepartmentNumber",
                value: "");

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Management",
                keyColumn: "Id",
                keyValue: new Guid("4575068a-4f0d-b6cd-8ea8-be680d8dc992"),
                column: "DepartmentNumber",
                value: "");

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "TestSlotStatus",
                keyColumn: "Id",
                keyValue: new Guid("215d0bfd-1ad4-4613-af9e-79d977dfd637"),
                column: "DisplayOrder",
                value: 4);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "TestSlotStatus",
                keyColumn: "Id",
                keyValue: new Guid("46ae567b-7f97-47ef-a960-9fd9efd32a53"),
                column: "DisplayOrder",
                value: 3);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "TestSlotStatus",
                keyColumn: "Id",
                keyValue: new Guid("cc71a094-b1b0-4c44-ab10-f48a8cc12140"),
                column: "DisplayOrder",
                value: 2);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "TestSlotStatus",
                keyColumn: "Id",
                keyValue: new Guid("fd397696-6596-4482-ae56-4c187813a65e"),
                column: "DisplayOrder",
                value: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            
            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -1615214602);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -1337878609);



            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -42408079);
            
            migrationBuilder.DropColumn(
                name: "Number",
                schema: "itv",
                table: "InterviewCommittee");

            migrationBuilder.AlterColumn<int>(
                name: "DepartmentNumber",
                schema: "lkp",
                table: "Management",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.InsertData(
                table: "AspNetRoleClaims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "RoleId" },
                values: new object[,]
                {
                    { -2029722188, "permission", "exams.create", new Guid("5f12e420-f666-4af4-a8fa-4e4aa755fdcd") },
                    { -1603357864, "permission", "exams.view", new Guid("5f12e420-f666-4af4-a8fa-4e4aa755fdcd") }
                });

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Management",
                keyColumn: "Id",
                keyValue: new Guid("453aa49d-8f16-a85b-9991-c8355ac8bf00"),
                column: "DepartmentNumber",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Management",
                keyColumn: "Id",
                keyValue: new Guid("4575068a-4f0d-b6cd-8ea8-be680d8dc992"),
                column: "DepartmentNumber",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "TestSlotStatus",
                keyColumn: "Id",
                keyValue: new Guid("215d0bfd-1ad4-4613-af9e-79d977dfd637"),
                column: "DisplayOrder",
                value: 6);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "TestSlotStatus",
                keyColumn: "Id",
                keyValue: new Guid("46ae567b-7f97-47ef-a960-9fd9efd32a53"),
                column: "DisplayOrder",
                value: 5);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "TestSlotStatus",
                keyColumn: "Id",
                keyValue: new Guid("cc71a094-b1b0-4c44-ab10-f48a8cc12140"),
                column: "DisplayOrder",
                value: 4);

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "TestSlotStatus",
                keyColumn: "Id",
                keyValue: new Guid("fd397696-6596-4482-ae56-4c187813a65e"),
                column: "DisplayOrder",
                value: 3);

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "TestSlotStatus",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsActive", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("02a5a982-e326-403c-a8da-34b2f33d88f0"), "Draft", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 1, true, false, "مسودة", "Draft", null, null },
                    { new Guid("4ff15b36-d258-48f4-ab1c-2380dfc0d1b0"), "Cancelled", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 7, true, false, "ملغي", "Cancelled", null, null },
                    { new Guid("7995eaf8-baaf-4dcc-a30d-0becc5a8e4d8"), "Approved", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 2, true, false, "معتمد", "Approved", null, null }
                });
        }
    }
}
