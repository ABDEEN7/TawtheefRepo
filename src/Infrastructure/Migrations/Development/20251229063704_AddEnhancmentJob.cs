using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Tawtheef.Infrastructure.Migrations.Development
{
    /// <inheritdoc />
    public partial class AddEnhancmentJob : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JobTabReviewAttachment",
                schema: "hr");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("2464b38a-5439-421d-ac39-e0bfbdcbc17e"));

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("bf879671-62de-4f17-8c07-38b6e6619fe0"));

            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                schema: "hr",
                table: "JobPointsMain",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "MaxPoints",
                schema: "hr",
                table: "JobPointConfiguration",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "JobReviewAttachment",
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
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReviewCycleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttachmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobReviewAttachment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobReviewAttachment_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobReviewAttachment_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobReviewAttachment_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobReviewAttachment_Job_JobId",
                        column: x => x.JobId,
                        principalSchema: "hr",
                        principalTable: "Job",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JobReviewAttachment_Resources_AttachmentId",
                        column: x => x.AttachmentId,
                        principalTable: "Resources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("12f5805d-6970-4a9e-a275-2b7cf3db3bb8"),
                column: "ConcurrencyStamp",
                value: "006ffe85-8432-4105-92a8-6511ba5f807e");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("1361d691-53c5-4a84-aea1-64ff134cf082"),
                column: "ConcurrencyStamp",
                value: "68937a91-013a-482b-8b17-5b69f1455eb5");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("5f12e420-f666-4af4-a8fa-4e4aa755fdcd"),
                column: "ConcurrencyStamp",
                value: "5d59a164-228d-4449-a7b1-7dd487fa4215");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("98e20970-b6bc-4da9-a947-f75e9adae3ca"),
                column: "ConcurrencyStamp",
                value: "9ab17cf9-0a52-438e-b36a-c102f94e3f7b");

            migrationBuilder.CreateIndex(
                name: "IX_JobReviewAttachment_AttachmentId",
                schema: "hr",
                table: "JobReviewAttachment",
                column: "AttachmentId");

            migrationBuilder.CreateIndex(
                name: "IX_JobReviewAttachment_CreatedById",
                schema: "hr",
                table: "JobReviewAttachment",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_JobReviewAttachment_DeletedById",
                schema: "hr",
                table: "JobReviewAttachment",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_JobReviewAttachment_JobId",
                schema: "hr",
                table: "JobReviewAttachment",
                column: "JobId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JobReviewAttachment_JobId_AttachmentId_ReviewCycleId",
                schema: "hr",
                table: "JobReviewAttachment",
                columns: new[] { "JobId", "AttachmentId", "ReviewCycleId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JobReviewAttachment_UpdatedById",
                schema: "hr",
                table: "JobReviewAttachment",
                column: "UpdatedById");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JobReviewAttachment",
                schema: "hr");

            migrationBuilder.DropColumn(
                name: "IsApproved",
                schema: "hr",
                table: "JobPointsMain");

            migrationBuilder.DropColumn(
                name: "MaxPoints",
                schema: "hr",
                table: "JobPointConfiguration");

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
                    AttachmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    JobTabReviewNoteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false)
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
                keyValue: new Guid("12f5805d-6970-4a9e-a275-2b7cf3db3bb8"),
                column: "ConcurrencyStamp",
                value: "ed6815e1-ff0b-4fca-8553-749f929295f8");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("1361d691-53c5-4a84-aea1-64ff134cf082"),
                column: "ConcurrencyStamp",
                value: "866e2ccb-ee08-43e7-a822-0563dedda52c");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("5f12e420-f666-4af4-a8fa-4e4aa755fdcd"),
                column: "ConcurrencyStamp",
                value: "158af8d9-5661-4f03-8da8-1ddbfc56720b");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("98e20970-b6bc-4da9-a947-f75e9adae3ca"),
                column: "ConcurrencyStamp",
                value: "859cf2e9-8352-4694-a843-a48e44da1bbd");

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "Avatar", "ConcurrencyStamp", "CreatedById", "CreatedDate", "CurrentAuthToken", "DeletedById", "DeletedDate", "Discriminator", "Email", "EmailConfirmed", "FullNameAr", "FullNameEn", "IsBlocked", "IsDeleted", "LastLoginDate", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "OtpAttempts", "OtpExpiry", "OtpReference", "OtpSends", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UpdatedById", "UpdatedDate", "UserName", "UserTypeId" },
                values: new object[,]
                {
                    { new Guid("2464b38a-5439-421d-ac39-e0bfbdcbc17e"), 0, null, "ae0628d3-8171-4a35-b168-fee762654e7a", null, new DateTimeOffset(new DateTime(1900, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 3, 0, 0, 0)), null, null, null, "EmployeeUser", "qa.e@tawtheef.com", true, "QA. E", "QA. E", false, false, null, false, null, "QA.E@TAWTHEEF.COM", "QA.E@TAWTHEEF.COM", 0, null, null, 0, "AQAAAAIAAYagAAAAEGcM+djZ3c2Q/N1kjZpDwcwjH2sfsRHkNS5H4lObrCaoiN238MHwgDFbLXiNsm5J4A==", null, false, "55669db7-ef3a-493f-89a6-a1d608403f83", false, null, null, "qa.e@tawtheef.com", new Guid("a1b2c3d4-e5f6-4879-8a3b-5c6d7e8f9a0b") },
                    { new Guid("bf879671-62de-4f17-8c07-38b6e6619fe0"), 0, null, "af24df87-26d1-4f94-b84b-410bbbe14859", null, new DateTimeOffset(new DateTime(1900, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 3, 0, 0, 0)), null, null, null, "ApplicantUser", "qa.a@tawtheef.com", true, "QA. A", "QA. A", false, false, null, false, null, "QA.A@TAWTHEEF.COM", "QA.A@TAWTHEEF.COM", 0, null, null, 0, "AQAAAAIAAYagAAAAEN8QCL2z2kO862Y8bQpTxN7RPyssbCDnnWOBERadOw0vaVF5WAL3D/axQfbD1BgXKA==", null, false, "18cc15bc-1783-40d9-a3b5-d26dd57c3f6c", false, null, null, "qa.a@tawtheef.com", new Guid("b2c3d4e5-f6a7-5984-9b2c-6d7e8f9a0b1c") }
                });

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
        }
    }
}
