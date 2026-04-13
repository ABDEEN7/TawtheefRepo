using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Tawtheef.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPermissionCities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                schema: "lkp",
                table: "Permission",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsActive", "IsAssignableToRole", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("a2d68b9a-89e4-555e-be1b-1a261a164899"), "cities.view", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 92, true, true, false, "المدن - عرض", "Cities - View", null, null },
                    { new Guid("f9d2c7ac-37a9-8b54-9cec-5bbd7c7f1019"), "cities.manage", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 93, true, true, false, "المدن - إدارة", "Cities - Manage", null, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("a2d68b9a-89e4-555e-be1b-1a261a164899"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("f9d2c7ac-37a9-8b54-9cec-5bbd7c7f1019"));
        }
    }
}
