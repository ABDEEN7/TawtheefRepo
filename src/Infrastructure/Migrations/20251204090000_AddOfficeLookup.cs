using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tawtheef.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOfficeLookup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Office",
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
                    CountryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    BackendName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DescriptionEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DescriptionAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Office", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Office_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Office_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Office_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Office_Country_CountryId",
                        column: x => x.CountryId,
                        principalSchema: "lkp",
                        principalTable: "Country",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.AddColumn<Guid>(
                name: "OfficeId",
                schema: "app",
                table: "UserProfile",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "Country",
                columns: new[] { "Id", "BackendName", "Code", "CodeAlpha", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "ISOCode", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("f58c33c6-5d13-4d29-8c81-7b52a56a62ab"), "Qatar", 974, "QAT", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "دولة قطر", "State of Qatar", 1, "QA", false, "قطر", "Qatar", null, null },
                    { new Guid("8f81a090-2e40-413b-bc37-ccba4b6a3b79"), "Egypt", 20, "EGY", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "جمهورية مصر العربية", "Arab Republic of Egypt", 2, "EG", false, "مصر", "Egypt", null, null }
                });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "Office",
                columns: new[] { "Id", "BackendName", "Code", "CountryId", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("9f335702-71a2-4188-b8f0-d2a52d0d6df6"), "DohaMainOffice", "QA-DOH-01", new Guid("f58c33c6-5d13-4d29-8c81-7b52a56a62ab"), null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "المكتب المعتمد الرئيسي في الدوحة", "Primary accredited office located in Doha", 1, false, "المكتب الرئيسي - الدوحة", "Doha Main Office", null, null },
                    { new Guid("3e6dfb62-96f4-4d95-96c5-38a8a454a269"), "CairoRegionalOffice", "EG-CAI-01", new Guid("8f81a090-2e40-413b-bc37-ccba4b6a3b79"), null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "مكتب معتمد إقليمي لخدمة المتقدمين خارج قطر", "Accredited regional office serving applicants outside Qatar", 2, false, "المكتب الإقليمي - القاهرة", "Cairo Regional Office", null, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Office_BackendName",
                schema: "lkp",
                table: "Office",
                column: "BackendName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Office_Code",
                schema: "lkp",
                table: "Office",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Office_CountryId",
                schema: "lkp",
                table: "Office",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Office_CreatedById",
                schema: "lkp",
                table: "Office",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Office_DeletedById",
                schema: "lkp",
                table: "Office",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_Office_DisplayOrder",
                schema: "lkp",
                table: "Office",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_Office_UpdatedById",
                schema: "lkp",
                table: "Office",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfile_OfficeId",
                schema: "app",
                table: "UserProfile",
                column: "OfficeId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfile_Office_OfficeId",
                schema: "app",
                table: "UserProfile",
                column: "OfficeId",
                principalSchema: "lkp",
                principalTable: "Office",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserProfile_Office_OfficeId",
                schema: "app",
                table: "UserProfile");

            migrationBuilder.DropTable(
                name: "Office",
                schema: "lkp");

            migrationBuilder.DropColumn(
                name: "OfficeId",
                schema: "app",
                table: "UserProfile");

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "Country",
                keyColumn: "Id",
                keyValue: new Guid("8f81a090-2e40-413b-bc37-ccba4b6a3b79"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "Country",
                keyColumn: "Id",
                keyValue: new Guid("f58c33c6-5d13-4d29-8c81-7b52a56a62ab"));
        }
    }
}
