#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Tawtheef.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOffice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProfileSkill_SkillId",
                schema: "pro",
                table: "ProfileSkill");

            migrationBuilder.DropIndex(
                name: "IX_ProfileLanguage_LanguageId",
                schema: "pro",
                table: "ProfileLanguage");

            migrationBuilder.AddColumn<Guid>(
                name: "OfficeAdminId",
                schema: "lkp",
                table: "Office",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "OfficeId",
                table: "AspNetUsers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsSystemRole",
                table: "AspNetRoles",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "JobPointConfiguration",
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
                    ApplicantCategoryMaxPoints = table.Column<int>(type: "int", nullable: false),
                    EducationMaxPoints = table.Column<int>(type: "int", nullable: false),
                    ExperienceMaxPoints = table.Column<int>(type: "int", nullable: false),
                    TrainingMaxPoints = table.Column<int>(type: "int", nullable: false),
                    CertificatesMaxPoints = table.Column<int>(type: "int", nullable: false),
                    SkillsMaxPoints = table.Column<int>(type: "int", nullable: false),
                    LanguagesMaxPoints = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobPointConfiguration", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobPointConfiguration_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobPointConfiguration_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobPointConfiguration_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "JobPointsMain",
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
                    ApplicantCategory = table.Column<int>(type: "int", nullable: false),
                    Education = table.Column<int>(type: "int", nullable: false),
                    Experience = table.Column<int>(type: "int", nullable: false),
                    Training = table.Column<int>(type: "int", nullable: false),
                    Certificates = table.Column<int>(type: "int", nullable: false),
                    Skills = table.Column<int>(type: "int", nullable: false),
                    Languages = table.Column<int>(type: "int", nullable: false),
                    Total = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobPointsMain", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobPointsMain_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobPointsMain_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobPointsMain_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobPointsMain_Job_JobId",
                        column: x => x.JobId,
                        principalSchema: "hr",
                        principalTable: "Job",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

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
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OfficeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CountryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OfficeSupportedCountry", x => x.Id);
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

            migrationBuilder.CreateTable(
                name: "JobPointsDetail",
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
                    JobPointsMainId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Points = table.Column<int>(type: "int", nullable: false),
                    ReferenceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobPointsDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobPointsDetail_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobPointsDetail_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobPointsDetail_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobPointsDetail_JobPointsMain_JobPointsMainId",
                        column: x => x.JobPointsMainId,
                        principalSchema: "hr",
                        principalTable: "JobPointsMain",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "DescriptionAr", "DescriptionEn", "IsSystemRole", "Name", "NameAr", "NameEn", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("12f5805d-6970-4a9e-a275-2b7cf3db3bb8"), "e9f5d7e0-c2e8-45ef-98e1-9be284e39f71", "موظف المكتب العادي", "Office user", true, "HRAdmin", "موظف المكتب", "Office User", "OFFICEUSER" },
                    { new Guid("1361d691-53c5-4a84-aea1-64ff134cf082"), "a35bd9ef-5815-4b98-b09a-680018847da3", "مدير النظام الكامل", "Full system administrator", true, "SystemAdmin", "مدير النظام", "System Admin", "SYSTEMADMIN" },
                    { new Guid("5f12e420-f666-4af4-a8fa-4e4aa755fdcd"), "1597dad4-73e5-4fca-a144-2f4bcfce588e", "مدير شؤون الموظفين", "Human resources administrator", true, "HRAdmin", "مدير الموارد البشرية", "HR Admin", "HRADMIN" },
                    { new Guid("98e20970-b6bc-4da9-a947-f75e9adae3ca"), "05a8384d-5efc-42ea-a485-74f39b352a4a", "مدير المكتب والصلاحيات المرتبطة", "Office administrator", true, "HRAdmin", "مدير المكتب", "Office Admin", "OFFICEADMIN" }
                });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "UserType",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[] { new Guid("5d1970fe-8398-44c0-88ea-560521933912"), "OfficeUser", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 4, false, "موظف مكتب", "OfficeUser", null, null });

            migrationBuilder.CreateIndex(
                name: "IX_ProfileSkill_SkillId_UserProfileId",
                schema: "pro",
                table: "ProfileSkill",
                columns: new[] { "SkillId", "UserProfileId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProfileLanguage_LanguageId_UserProfileId",
                schema: "pro",
                table: "ProfileLanguage",
                columns: new[] { "LanguageId", "UserProfileId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Office_OfficeAdminId",
                schema: "lkp",
                table: "Office",
                column: "OfficeAdminId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_OfficeId",
                table: "AspNetUsers",
                column: "OfficeId");

            migrationBuilder.CreateIndex(
                name: "IX_JobPointConfiguration_CreatedById",
                schema: "hr",
                table: "JobPointConfiguration",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_JobPointConfiguration_DeletedById",
                schema: "hr",
                table: "JobPointConfiguration",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_JobPointConfiguration_UpdatedById",
                schema: "hr",
                table: "JobPointConfiguration",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_JobPointsDetail_CreatedById",
                schema: "hr",
                table: "JobPointsDetail",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_JobPointsDetail_DeletedById",
                schema: "hr",
                table: "JobPointsDetail",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_JobPointsDetail_JobPointsMainId",
                schema: "hr",
                table: "JobPointsDetail",
                column: "JobPointsMainId");

            migrationBuilder.CreateIndex(
                name: "IX_JobPointsDetail_UpdatedById",
                schema: "hr",
                table: "JobPointsDetail",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_JobPointsMain_CreatedById",
                schema: "hr",
                table: "JobPointsMain",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_JobPointsMain_DeletedById",
                schema: "hr",
                table: "JobPointsMain",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_JobPointsMain_JobId",
                schema: "hr",
                table: "JobPointsMain",
                column: "JobId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JobPointsMain_UpdatedById",
                schema: "hr",
                table: "JobPointsMain",
                column: "UpdatedById");

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
                name: "IX_OfficeSupportedCountry_OfficeId",
                schema: "lkp",
                table: "OfficeSupportedCountry",
                column: "OfficeId");

            migrationBuilder.CreateIndex(
                name: "IX_OfficeSupportedCountry_UpdatedById",
                schema: "lkp",
                table: "OfficeSupportedCountry",
                column: "UpdatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Office_OfficeId",
                table: "AspNetUsers",
                column: "OfficeId",
                principalSchema: "lkp",
                principalTable: "Office",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Office_AspNetUsers_OfficeAdminId",
                schema: "lkp",
                table: "Office",
                column: "OfficeAdminId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Office_OfficeId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Office_AspNetUsers_OfficeAdminId",
                schema: "lkp",
                table: "Office");

            migrationBuilder.DropTable(
                name: "JobPointConfiguration",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "JobPointsDetail",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "OfficeSupportedCountry",
                schema: "lkp");

            migrationBuilder.DropTable(
                name: "JobPointsMain",
                schema: "hr");

            migrationBuilder.DropIndex(
                name: "IX_ProfileSkill_SkillId_UserProfileId",
                schema: "pro",
                table: "ProfileSkill");

            migrationBuilder.DropIndex(
                name: "IX_ProfileLanguage_LanguageId_UserProfileId",
                schema: "pro",
                table: "ProfileLanguage");

            migrationBuilder.DropIndex(
                name: "IX_Office_OfficeAdminId",
                schema: "lkp",
                table: "Office");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_OfficeId",
                table: "AspNetUsers");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("12f5805d-6970-4a9e-a275-2b7cf3db3bb8"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("1361d691-53c5-4a84-aea1-64ff134cf082"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("5f12e420-f666-4af4-a8fa-4e4aa755fdcd"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("98e20970-b6bc-4da9-a947-f75e9adae3ca"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "UserType",
                keyColumn: "Id",
                keyValue: new Guid("5d1970fe-8398-44c0-88ea-560521933912"));

            migrationBuilder.DropColumn(
                name: "OfficeAdminId",
                schema: "lkp",
                table: "Office");

            migrationBuilder.DropColumn(
                name: "OfficeId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IsSystemRole",
                table: "AspNetRoles");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileSkill_SkillId",
                schema: "pro",
                table: "ProfileSkill",
                column: "SkillId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileLanguage_LanguageId",
                schema: "pro",
                table: "ProfileLanguage",
                column: "LanguageId");
        }
    }
}
