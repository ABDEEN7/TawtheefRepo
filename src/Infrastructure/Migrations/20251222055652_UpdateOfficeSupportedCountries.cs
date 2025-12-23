using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Tawtheef.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateOfficeSupportedCountries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ResidentBreakdown",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "JobQuota",
                schema: "hr");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OfficeSupportedCountry",
                schema: "lkp",
                table: "OfficeSupportedCountry");

            migrationBuilder.DropColumn(
                name: "BackendName",
                schema: "lkp",
                table: "OfficeSupportedCountry");

            migrationBuilder.DropColumn(
                name: "DescriptionAr",
                schema: "lkp",
                table: "OfficeSupportedCountry");

            migrationBuilder.DropColumn(
                name: "DescriptionEn",
                schema: "lkp",
                table: "OfficeSupportedCountry");

            migrationBuilder.DropColumn(
                name: "DisplayOrder",
                schema: "lkp",
                table: "OfficeSupportedCountry");

            migrationBuilder.DropColumn(
                name: "NameAr",
                schema: "lkp",
                table: "OfficeSupportedCountry");

            migrationBuilder.DropColumn(
                name: "NameEn",
                schema: "lkp",
                table: "OfficeSupportedCountry");

            migrationBuilder.AlterColumn<bool>(
                name: "IsSystemRole",
                table: "AspNetRoles",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_OfficeSupportedCountry",
                schema: "lkp",
                table: "OfficeSupportedCountry",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "JobTabReviewNote",
                schema: "hr",
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
                    JobId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Tab = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    TabStatus = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    IsResolved = table.Column<bool>(type: "bit", nullable: false),
                    ReviewCycleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobTabReviewNote", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobTabReviewNote_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobTabReviewNote_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobTabReviewNote_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobTabReviewNote_Job_JobId",
                        column: x => x.JobId,
                        principalSchema: "hr",
                        principalTable: "Job",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "JobTabReviewAttachment",
                schema: "hr",
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
                    JobTabReviewNoteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AttachmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobTabReviewAttachment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobTabReviewAttachment_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobTabReviewAttachment_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobTabReviewAttachment_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobTabReviewAttachment_JobTabReviewNote_JobTabReviewNoteId",
                        column: x => x.JobTabReviewNoteId,
                        principalSchema: "hr",
                        principalTable: "JobTabReviewNote",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobTabReviewAttachment_Resources_AttachmentId",
                        column: x => x.AttachmentId,
                        principalTable: "Resources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("1361d691-53c5-4a84-aea1-64ff134cf082"),
                columns: new[] { "ConcurrencyStamp", "NormalizedName" },
                values: new object[] { "bda2236c-3cf0-4803-9c42-f88593b23a18", "SystemAdmin" });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("5f12e420-f666-4af4-a8fa-4e4aa755fdcd"),
                columns: new[] { "ConcurrencyStamp", "NormalizedName" },
                values: new object[] { "eda92d98-8077-432e-911e-ac4c72cce0a5", "HRAdmin" });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("98e20970-b6bc-4da9-a947-f75e9adae3ca"),
                columns: new[] { "ConcurrencyStamp", "NormalizedName" },
                values: new object[] { "d11c2b48-8b06-4d7a-9080-952675794347", "OfficeAdmin" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "DescriptionAr", "DescriptionEn", "IsSystemRole", "Name", "NameAr", "NameEn", "NormalizedName" },
                values: new object[] { new Guid("12f5805d-6970-4a9e-a275-2b7cf3db3bb8"), "9bc357a0-c1c0-4263-8f16-a3c9aa83f917", "موظف المكتب العادي", "Office user", true, "OfficeUser", "موظف المكتب", "Office User", "OfficeUser" });

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "JobStatus",
                keyColumn: "Id",
                keyValue: new Guid("e07bd71f-c466-0eea-49cc-2b13d1d9403f"),
                columns: new[] { "DescriptionAr", "NameAr" },
                values: new object[] { "الوظيفة قيد الأعتماد.", "قيد الأعتماد" });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "JobStatus",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[] { new Guid("e0cd7b22-8948-0c37-9b15-2e5217f00065"), "NeedUpdate", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "الوظيفة تحتاج للتعديل", "Job needs to be updated.", 1, false, "تحتاج للتعديل", "Need Update", null, null });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "SkillType",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("2f1b6ce7-cbc3-2b5c-b264-a02c6a87be1d"), "Educational", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "مهارات تعليمية تم الحصول عليها من خلال التعليم الرسمي", "Educational skills acquired through formal education", 1, false, "تعليمي", "Educational", null, null },
                    { new Guid("83b504d0-25c1-5ca0-6757-df299869f002"), "Professional", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "مهارات مهنية ناعمة وكفاءات مكان العمل", "Professional soft skills and workplace competencies", 3, false, "مهني", "Professional", null, null },
                    { new Guid("d14ac141-c057-16f5-ddd8-97d02f6a7c9b"), "Technical", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "مهارات تقنية أو صلبة تتعلق بأدوات أو تقنيات أو منهجيات محددة", "Technical or hard skills related to specific tools, technologies, or methodologies", 2, false, "تقني", "Technical", null, null },
                    { new Guid("f91eb9e6-7a3f-76d6-1fe1-4443cecc5b9a"), "Other", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "أنواع أخرى من المهارات غير المصنفة أعلاه", "Other types of skills not categorized above", 4, false, "أخرى", "Other", null, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_OfficeSupportedCountry_OfficeId",
                schema: "lkp",
                table: "OfficeSupportedCountry",
                column: "OfficeId");

            migrationBuilder.CreateIndex(
                name: "IX_JobTabReviewAttachment_AttachmentId",
                schema: "hr",
                table: "JobTabReviewAttachment",
                column: "AttachmentId");

            migrationBuilder.CreateIndex(
                name: "IX_JobTabReviewAttachment_CreatedById",
                schema: "hr",
                table: "JobTabReviewAttachment",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_JobTabReviewAttachment_DeletedById",
                schema: "hr",
                table: "JobTabReviewAttachment",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_JobTabReviewAttachment_JobTabReviewNoteId",
                schema: "hr",
                table: "JobTabReviewAttachment",
                column: "JobTabReviewNoteId");

            migrationBuilder.CreateIndex(
                name: "IX_JobTabReviewAttachment_UpdatedById",
                schema: "hr",
                table: "JobTabReviewAttachment",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_JobTabReviewNote_CreatedById",
                schema: "hr",
                table: "JobTabReviewNote",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_JobTabReviewNote_DeletedById",
                schema: "hr",
                table: "JobTabReviewNote",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_JobTabReviewNote_JobId_Tab_ReviewCycleId",
                schema: "hr",
                table: "JobTabReviewNote",
                columns: new[] { "JobId", "Tab", "ReviewCycleId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JobTabReviewNote_UpdatedById",
                schema: "hr",
                table: "JobTabReviewNote",
                column: "UpdatedById");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JobTabReviewAttachment",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "JobTabReviewNote",
                schema: "hr");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OfficeSupportedCountry",
                schema: "lkp",
                table: "OfficeSupportedCountry");

            migrationBuilder.DropIndex(
                name: "IX_OfficeSupportedCountry_OfficeId",
                schema: "lkp",
                table: "OfficeSupportedCountry");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("12f5805d-6970-4a9e-a275-2b7cf3db3bb8"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "JobStatus",
                keyColumn: "Id",
                keyValue: new Guid("e0cd7b22-8948-0c37-9b15-2e5217f00065"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "SkillType",
                keyColumn: "Id",
                keyValue: new Guid("2f1b6ce7-cbc3-2b5c-b264-a02c6a87be1d"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "SkillType",
                keyColumn: "Id",
                keyValue: new Guid("83b504d0-25c1-5ca0-6757-df299869f002"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "SkillType",
                keyColumn: "Id",
                keyValue: new Guid("d14ac141-c057-16f5-ddd8-97d02f6a7c9b"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "SkillType",
                keyColumn: "Id",
                keyValue: new Guid("f91eb9e6-7a3f-76d6-1fe1-4443cecc5b9a"));

            migrationBuilder.AddColumn<string>(
                name: "BackendName",
                schema: "lkp",
                table: "OfficeSupportedCountry",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DescriptionAr",
                schema: "lkp",
                table: "OfficeSupportedCountry",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DescriptionEn",
                schema: "lkp",
                table: "OfficeSupportedCountry",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DisplayOrder",
                schema: "lkp",
                table: "OfficeSupportedCountry",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "NameAr",
                schema: "lkp",
                table: "OfficeSupportedCountry",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NameEn",
                schema: "lkp",
                table: "OfficeSupportedCountry",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<bool>(
                name: "IsSystemRole",
                table: "AspNetRoles",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OfficeSupportedCountry",
                schema: "lkp",
                table: "OfficeSupportedCountry",
                columns: new[] { "OfficeId", "CountryId" });

            migrationBuilder.CreateTable(
                name: "JobQuota",
                schema: "hr",
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
                    JobId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Gcc = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    NonQatariSpouse = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    QatarMother = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    QatariCitizens = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    QuGrads = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Residents = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobQuota", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobQuota_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobQuota_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobQuota_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobQuota_Job_JobId",
                        column: x => x.JobId,
                        principalSchema: "hr",
                        principalTable: "Job",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ResidentBreakdown",
                schema: "hr",
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
                    JobQuotaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NationalityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Percentage = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResidentBreakdown", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResidentBreakdown_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ResidentBreakdown_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ResidentBreakdown_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ResidentBreakdown_Country_NationalityId",
                        column: x => x.NationalityId,
                        principalSchema: "lkp",
                        principalTable: "Country",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ResidentBreakdown_JobQuota_JobQuotaId",
                        column: x => x.JobQuotaId,
                        principalSchema: "hr",
                        principalTable: "JobQuota",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("1361d691-53c5-4a84-aea1-64ff134cf082"),
                columns: new[] { "ConcurrencyStamp", "NormalizedName" },
                values: new object[] { "2a103505-c338-43cf-9a96-00f37af0578f", "SYSTEMADMIN" });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("5f12e420-f666-4af4-a8fa-4e4aa755fdcd"),
                columns: new[] { "ConcurrencyStamp", "NormalizedName" },
                values: new object[] { "a02a5c44-a6e5-41b9-b4f2-b38a0e50fe39", "HRADMIN" });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("98e20970-b6bc-4da9-a947-f75e9adae3ca"),
                columns: new[] { "ConcurrencyStamp", "NormalizedName" },
                values: new object[] { "9e922ea2-a9b2-4255-94a7-1ff8b4591765", "OFFICEADMIN" });

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "JobStatus",
                keyColumn: "Id",
                keyValue: new Guid("e07bd71f-c466-0eea-49cc-2b13d1d9403f"),
                columns: new[] { "DescriptionAr", "NameAr" },
                values: new object[] { "الوظيفة قيد الموافقة.", "قيد الموافقة" });

            migrationBuilder.CreateIndex(
                name: "IX_JobQuota_CreatedById",
                schema: "hr",
                table: "JobQuota",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_JobQuota_DeletedById",
                schema: "hr",
                table: "JobQuota",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_JobQuota_JobId",
                schema: "hr",
                table: "JobQuota",
                column: "JobId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JobQuota_UpdatedById",
                schema: "hr",
                table: "JobQuota",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ResidentBreakdown_CreatedById",
                schema: "hr",
                table: "ResidentBreakdown",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ResidentBreakdown_DeletedById",
                schema: "hr",
                table: "ResidentBreakdown",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_ResidentBreakdown_JobQuotaId",
                schema: "hr",
                table: "ResidentBreakdown",
                column: "JobQuotaId");

            migrationBuilder.CreateIndex(
                name: "IX_ResidentBreakdown_NationalityId",
                schema: "hr",
                table: "ResidentBreakdown",
                column: "NationalityId");

            migrationBuilder.CreateIndex(
                name: "IX_ResidentBreakdown_UpdatedById",
                schema: "hr",
                table: "ResidentBreakdown",
                column: "UpdatedById");
        }
    }
}
