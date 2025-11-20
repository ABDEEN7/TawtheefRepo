using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tawtheef.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddNewOptionGender : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                schema: "lkp",
                table: "Gender",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[] { new Guid("4436a8ce-3f1e-4581-be3d-839c67127a9f"), "All", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "جميع الخيارات", "All Options", 0, false, "جميعها", "All", null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "Gender",
                keyColumn: "Id",
                keyValue: new Guid("4436a8ce-3f1e-4581-be3d-839c67127a9f"));
        }
    }
}
