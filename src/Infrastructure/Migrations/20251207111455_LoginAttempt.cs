using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tawtheef.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class LoginAttempt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LoginAttempt",
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
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Source = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Succeeded = table.Column<bool>(type: "bit", nullable: false),
                    FailureReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SessionId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AttemptedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoginAttempt", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LoginAttempt_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LoginAttempt_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LoginAttempt_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LoginAttempt_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LoginAttempt_UserType_UserTypeId",
                        column: x => x.UserTypeId,
                        principalSchema: "lkp",
                        principalTable: "UserType",
                        principalColumn: "Id");
                });

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "LanguageLevel",
                keyColumn: "Id",
                keyValue: new Guid("f0c9e84a-258f-4f6b-a469-153a92d68730"),
                column: "NameAr",
                value: "مبتدئ");

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "LanguageLevel",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[] { new Guid("5161f23a-f501-414c-b4fe-81cde9ebcd5d"), "Expert", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 4, false, "خبير", "Expert", null, null });

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "RatingGrade",
                keyColumn: "Id",
                keyValue: new Guid("2cf3d4e2-33cd-4671-9667-1b5e6e0cee9a"),
                columns: new[] { "BackendName", "NameAr", "NameEn" },
                values: new object[] { "Intermediate", "متوسط", "Intermediate" });

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "RatingGrade",
                keyColumn: "Id",
                keyValue: new Guid("4dbd3381-f57a-4f6c-a718-432970d32276"),
                columns: new[] { "BackendName", "NameAr", "NameEn" },
                values: new object[] { "Expert", "خبير", "Expert" });

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "RatingGrade",
                keyColumn: "Id",
                keyValue: new Guid("69c0943a-f04f-4b6c-9145-1fbfae4b5c2e"),
                columns: new[] { "BackendName", "NameAr", "NameEn" },
                values: new object[] { "Basic", "أساسي", "Basic" });

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "RatingGrade",
                keyColumn: "Id",
                keyValue: new Guid("b8f10518-bf02-4357-a0e3-1f6bfb1746cd"),
                columns: new[] { "BackendName", "NameAr", "NameEn" },
                values: new object[] { "Advanced", "متقدم", "Advanced" });

            migrationBuilder.CreateIndex(
                name: "IX_LoginAttempt_CreatedById",
                table: "LoginAttempt",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_LoginAttempt_DeletedById",
                table: "LoginAttempt",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_LoginAttempt_UpdatedById",
                table: "LoginAttempt",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_LoginAttempt_UserId",
                table: "LoginAttempt",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_LoginAttempt_UserTypeId",
                table: "LoginAttempt",
                column: "UserTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LoginAttempt");

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "LanguageLevel",
                keyColumn: "Id",
                keyValue: new Guid("5161f23a-f501-414c-b4fe-81cde9ebcd5d"));

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "LanguageLevel",
                keyColumn: "Id",
                keyValue: new Guid("f0c9e84a-258f-4f6b-a469-153a92d68730"),
                column: "NameAr",
                value: "أساسي");

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "RatingGrade",
                keyColumn: "Id",
                keyValue: new Guid("2cf3d4e2-33cd-4671-9667-1b5e6e0cee9a"),
                columns: new[] { "BackendName", "NameAr", "NameEn" },
                values: new object[] { "Good", "جيد", "Good" });

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "RatingGrade",
                keyColumn: "Id",
                keyValue: new Guid("4dbd3381-f57a-4f6c-a718-432970d32276"),
                columns: new[] { "BackendName", "NameAr", "NameEn" },
                values: new object[] { "Excellent", "ممتاز", "Excellent" });

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "RatingGrade",
                keyColumn: "Id",
                keyValue: new Guid("69c0943a-f04f-4b6c-9145-1fbfae4b5c2e"),
                columns: new[] { "BackendName", "NameAr", "NameEn" },
                values: new object[] { "Acceptable", "مقبول", "Acceptable" });

            migrationBuilder.UpdateData(
                schema: "lkp",
                table: "RatingGrade",
                keyColumn: "Id",
                keyValue: new Guid("b8f10518-bf02-4357-a0e3-1f6bfb1746cd"),
                columns: new[] { "BackendName", "NameAr", "NameEn" },
                values: new object[] { "VeryGood", "جيد جدًا", "Very Good" });
        }
    }
}
