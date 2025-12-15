using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tawtheef.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOfficeSupportedCountry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Country_Office_OfficeId",
                schema: "lkp",
                table: "Country");

            migrationBuilder.DropIndex(
                name: "IX_Country_OfficeId",
                schema: "lkp",
                table: "Country");

            migrationBuilder.DropColumn(
                name: "OfficeId",
                schema: "lkp",
                table: "Country");

            migrationBuilder.CreateTable(
                name: "OfficeSupportedCountry",
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
                    OfficeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CountryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BackendName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DescriptionAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DescriptionEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OfficeSupportedCountry", x => new { x.OfficeId, x.CountryId });
                    table.ForeignKey(
                        name: "FK_OfficeSupportedCountry_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OfficeSupportedCountry_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OfficeSupportedCountry_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OfficeSupportedCountry_Country_CountryId",
                        column: x => x.CountryId,
                        principalSchema: "lkp",
                        principalTable: "Country",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OfficeSupportedCountry_Office_OfficeId",
                        column: x => x.OfficeId,
                        principalSchema: "lkp",
                        principalTable: "Office",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("1361d691-53c5-4a84-aea1-64ff134cf082"),
                column: "ConcurrencyStamp",
                value: "2a103505-c338-43cf-9a96-00f37af0578f");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("5f12e420-f666-4af4-a8fa-4e4aa755fdcd"),
                column: "ConcurrencyStamp",
                value: "a02a5c44-a6e5-41b9-b4f2-b38a0e50fe39");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("98e20970-b6bc-4da9-a947-f75e9adae3ca"),
                column: "ConcurrencyStamp",
                value: "9e922ea2-a9b2-4255-94a7-1ff8b4591765");

            migrationBuilder.CreateIndex(
                name: "IX_OfficeSupportedCountry_CountryId",
                schema: "lkp",
                table: "OfficeSupportedCountry",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_OfficeSupportedCountry_CreatedById",
                schema: "lkp",
                table: "OfficeSupportedCountry",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_OfficeSupportedCountry_DeletedById",
                schema: "lkp",
                table: "OfficeSupportedCountry",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_OfficeSupportedCountry_UpdatedById",
                schema: "lkp",
                table: "OfficeSupportedCountry",
                column: "UpdatedById");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OfficeSupportedCountry",
                schema: "lkp");

            migrationBuilder.AddColumn<Guid>(
                name: "OfficeId",
                schema: "lkp",
                table: "Country",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("1361d691-53c5-4a84-aea1-64ff134cf082"),
                column: "ConcurrencyStamp",
                value: "12990d2f-1d47-4058-ab89-e77c0fded7de");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("5f12e420-f666-4af4-a8fa-4e4aa755fdcd"),
                column: "ConcurrencyStamp",
                value: "2e6eab95-bad1-433c-9e7d-e9822f05be63");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("98e20970-b6bc-4da9-a947-f75e9adae3ca"),
                column: "ConcurrencyStamp",
                value: "cb109cbd-1fbf-4a19-a19b-9750f1562ffb");

            migrationBuilder.CreateIndex(
                name: "IX_Country_OfficeId",
                schema: "lkp",
                table: "Country",
                column: "OfficeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Country_Office_OfficeId",
                schema: "lkp",
                table: "Country",
                column: "OfficeId",
                principalSchema: "lkp",
                principalTable: "Office",
                principalColumn: "Id");
        }
    }
}
