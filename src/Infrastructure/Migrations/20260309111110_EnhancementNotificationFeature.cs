using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Tawtheef.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EnhancementNotificationFeature : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("5f12e420-f666-4af4-a8fa-4e4aa755fdcd"), new Guid("0593ad82-e44e-4f55-aa08-c5c80764a873") });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("d8689e0c-d872-42e9-87b7-c3bb27305e07"), new Guid("0593ad82-e44e-4f55-aa08-c5c80764a873") });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("5f12e420-f666-4af4-a8fa-4e4aa755fdcd"), new Guid("42e0d563-7603-453c-81b1-6b2325622b40") });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("d8689e0c-d872-42e9-87b7-c3bb27305e07"), new Guid("42e0d563-7603-453c-81b1-6b2325622b40") });

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("0593ad82-e44e-4f55-aa08-c5c80764a873"));

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("42e0d563-7603-453c-81b1-6b2325622b40"));

            migrationBuilder.AddColumn<string>(
                name: "IdempotencyKey",
                table: "Notifications",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaxRetries",
                table: "Notifications",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "NextRetryAt",
                table: "Notifications",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PlainTextBody",
                table: "Notifications",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RetryCount",
                table: "Notifications",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_IdempotencyKey",
                table: "Notifications",
                column: "IdempotencyKey",
                unique: true,
                filter: "[IdempotencyKey] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_NextRetryAt",
                table: "Notifications",
                column: "NextRetryAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Notifications_IdempotencyKey",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_NextRetryAt",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "IdempotencyKey",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "MaxRetries",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "NextRetryAt",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "PlainTextBody",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "RetryCount",
                table: "Notifications");

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "Avatar", "ConcurrencyStamp", "CreatedById", "CreatedDate", "CurrentAuthToken", "DeletedById", "DeletedDate", "Discriminator", "Email", "EmailConfirmed", "EmployeeProfileId", "FullNameAr", "FullNameEn", "IsBlocked", "IsDeleted", "LastLoginDate", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "OtpAttempts", "OtpExpiry", "OtpLockedUntilUtc", "OtpReference", "OtpSendWindowStartUtc", "OtpSendsInWindow", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UpdatedById", "UpdatedDate", "UserName", "UserTypeId" },
                values: new object[,]
                {
                    { new Guid("0593ad82-e44e-4f55-aa08-c5c80764a873"), 0, null, "75a677a7-c93d-4940-8666-4d648343104c", null, new DateTime(1900, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, "EmployeeUser", "t-m.khatatbeh@edu.gov.qa", true, null, "t-m.khatatbeh", "t-m.khatatbeh", false, false, null, false, null, "T-M.KHATATBEH@EDU.GOV.QA", "T-M.KHATATBEH@EDU.GOV.QA", 0, null, null, null, null, 0, null, null, false, "a984b6f5-e904-44b0-8d0d-5e93c07b1510", false, null, null, "t-m.khatatbeh@edu.gov.qa", new Guid("a1b2c3d4-e5f6-4879-8a3b-5c6d7e8f9a0b") },
                    { new Guid("42e0d563-7603-453c-81b1-6b2325622b40"), 0, null, "75a677a7-c93d-4940-8666-4d648343104c", null, new DateTime(1900, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, "EmployeeUser", "t-hu.ahmed@edu.gov.qa", true, null, "t-hu.ahmed", "t-hu.ahmed", false, false, null, false, null, "T-HU.AHMED@EDU.GOV.QA", "T-HU.AHMED@EDU.GOV.QA", 0, null, null, null, null, 0, null, null, false, "a984b6f5-e904-44b0-8d0d-5e93c07b1510", false, null, null, "t-hu.ahmed@edu.gov.qa", new Guid("a1b2c3d4-e5f6-4879-8a3b-5c6d7e8f9a0b") }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { new Guid("5f12e420-f666-4af4-a8fa-4e4aa755fdcd"), new Guid("0593ad82-e44e-4f55-aa08-c5c80764a873") },
                    { new Guid("d8689e0c-d872-42e9-87b7-c3bb27305e07"), new Guid("0593ad82-e44e-4f55-aa08-c5c80764a873") },
                    { new Guid("5f12e420-f666-4af4-a8fa-4e4aa755fdcd"), new Guid("42e0d563-7603-453c-81b1-6b2325622b40") },
                    { new Guid("d8689e0c-d872-42e9-87b7-c3bb27305e07"), new Guid("42e0d563-7603-453c-81b1-6b2325622b40") }
                });
        }
    }
}
