using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tawtheef.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixDeptNoType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "DepartmentNumber",
                schema: "lkp",
                table: "Management",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

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
        }
    }
}
