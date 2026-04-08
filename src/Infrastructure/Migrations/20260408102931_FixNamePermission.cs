using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tawtheef.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixNamePermission : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("4cac1167-25a0-5d5f-9d34-9e42c3a3b51d"),
                column: "NameAr",
                value: "مكتب سعادة الوزير - عرض");

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("f12b680a-44c9-b955-9de8-5e22fd77e601"),
                column: "NameAr",
                value: "مكتب سعادة الوزير - إدارة");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("4cac1167-25a0-5d5f-9d34-9e42c3a3b51d"),
                column: "NameAr",
                value: "مكتب الوزير - عرض");

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("f12b680a-44c9-b955-9de8-5e22fd77e601"),
                column: "NameAr",
                value: "مكتب الوزير - إدارة");
        }
    }
}
