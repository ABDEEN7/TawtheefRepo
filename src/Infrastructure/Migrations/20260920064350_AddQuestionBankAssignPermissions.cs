using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tawtheef.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddQuestionBankAssignPermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoleClaims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "RoleId" },
                values: new object[] { -2132548858, "permission", "question-bank-requests.assign", new Guid("5b757ede-93e1-4c7a-88d3-5e89007d648b") });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "Permission",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsActive", "IsAssignableToRole", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[] { new Guid("e832e3df-8c43-9d51-add9-104242b1e56a"), "question-bank-requests.assign", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 87, true, true, false, "طلبات بنوك الأسئلة - إسناد", "Question Bank Requests - Assign", null, null });

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionBankRequestHistory_AspNetUsers_PerformedById",
                schema: "hr",
                table: "QuestionBankRequestHistory",
                column: "PerformedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionBankVersion_AspNetUsers_ApprovedById",
                schema: "hr",
                table: "QuestionBankVersion",
                column: "ApprovedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionBankVersionChange_AspNetUsers_ChangedById",
                schema: "hr",
                table: "QuestionBankVersionChange",
                column: "ChangedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuestionBankRequestHistory_AspNetUsers_PerformedById",
                schema: "hr",
                table: "QuestionBankRequestHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionBankVersion_AspNetUsers_ApprovedById",
                schema: "hr",
                table: "QuestionBankVersion");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionBankVersionChange_AspNetUsers_ChangedById",
                schema: "hr",
                table: "QuestionBankVersionChange");

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -2132548858);

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("e832e3df-8c43-9d51-add9-104242b1e56a"));
        }
    }
}
