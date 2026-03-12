using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Tawtheef.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RolePermissionCorrection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("1361d691-53c5-4a84-aea1-64ff134cf082"), new Guid("48f6d7a2-e821-4ab6-81cf-884ed650d2ec") });

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("48f6d7a2-e821-4ab6-81cf-884ed650d2ec"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "UserType",
                keyColumn: "Id",
                keyValue: new Guid("c3d4e5f6-a7b8-6a95-0c3d-7e8f9a0b1c2d"));

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -82,
                column: "ClaimValue",
                value: "candidate.users.manage");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -81,
                column: "ClaimValue",
                value: "candidate.users.view");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -80,
                column: "ClaimValue",
                value: "office.users.manage");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -79,
                column: "ClaimValue",
                value: "office.users.view");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -78,
                column: "ClaimValue",
                value: "kawader.manage");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -77,
                column: "ClaimValue",
                value: "nominations.manage");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -76,
                column: "ClaimValue",
                value: "nominations.view");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -75,
                column: "ClaimValue",
                value: "jobs.invitations.view");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -74,
                column: "ClaimValue",
                value: "jobs.points.approve");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -73,
                column: "ClaimValue",
                value: "jobs.points.view");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -72,
                column: "ClaimValue",
                value: "jobs.points.manage");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -71,
                column: "ClaimValue",
                value: "jobs.approve");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -70,
                column: "ClaimValue",
                value: "jobs.manage");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -69,
                column: "ClaimValue",
                value: "jobs.view");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -68,
                column: "ClaimValue",
                value: "profile.approval.changes");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -67,
                column: "ClaimValue",
                value: "profile.approval.review");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -66,
                column: "ClaimValue",
                value: "profile.approval.view");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -65,
                column: "ClaimValue",
                value: "profile.distribution.manage");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -64,
                column: "ClaimValue",
                value: "profile.distribution.view");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -63,
                column: "ClaimValue",
                value: "profile.manage");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -62,
                column: "ClaimValue",
                value: "profile.view");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -61,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "profile.approval.changes", new Guid("12f5805d-6970-4a9e-a275-2b7cf3db3bb8") });

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -60,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "profile.approval.review", new Guid("12f5805d-6970-4a9e-a275-2b7cf3db3bb8") });

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -59,
                column: "ClaimValue",
                value: "profile.approval.view");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -58,
                column: "ClaimValue",
                value: "profile.manage");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -57,
                column: "ClaimValue",
                value: "profile.view");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -56,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "profile.approval.changes", new Guid("98e20970-b6bc-4da9-a947-f75e9adae3ca") });

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -55,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "profile.approval.review", new Guid("98e20970-b6bc-4da9-a947-f75e9adae3ca") });

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -54,
                column: "ClaimValue",
                value: "profile.approval.view");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -53,
                column: "ClaimValue",
                value: "profile.distribution.manage");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -52,
                column: "ClaimValue",
                value: "profile.distribution.view");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -51,
                column: "ClaimValue",
                value: "profile.manage");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -50,
                column: "ClaimValue",
                value: "profile.view");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -49,
                column: "ClaimValue",
                value: "office.users.manage");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -48,
                column: "ClaimValue",
                value: "office.users.view");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -47,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "candidate.users.view", new Guid("ac011a30-6b0e-496c-a8ef-ba8132bd1808") });

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -46,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "nominations.view", new Guid("ac011a30-6b0e-496c-a8ef-ba8132bd1808") });

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -45,
                column: "ClaimValue",
                value: "jobs.points.approve");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -44,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "jobs.points.view", new Guid("ac011a30-6b0e-496c-a8ef-ba8132bd1808") });

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -43,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "jobs.approve", new Guid("ac011a30-6b0e-496c-a8ef-ba8132bd1808") });

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -42,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "jobs.view", new Guid("ac011a30-6b0e-496c-a8ef-ba8132bd1808") });

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -41,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "profile.approval.review", new Guid("ac011a30-6b0e-496c-a8ef-ba8132bd1808") });

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -40,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "profile.approval.view", new Guid("ac011a30-6b0e-496c-a8ef-ba8132bd1808") });

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -39,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "profile.view", new Guid("ac011a30-6b0e-496c-a8ef-ba8132bd1808") });

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -38,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "dashboard.view", new Guid("ac011a30-6b0e-496c-a8ef-ba8132bd1808") });

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -37,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "profile.logs.view", new Guid("5b757ede-93e1-4c7a-88d3-5e89007d648b") });

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -36,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "major-skill.management", new Guid("5b757ede-93e1-4c7a-88d3-5e89007d648b") });

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -35,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "organization-structures.manage", new Guid("5b757ede-93e1-4c7a-88d3-5e89007d648b") });

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -34,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "candidate.users.manage", new Guid("5b757ede-93e1-4c7a-88d3-5e89007d648b") });

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -33,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "candidate.users.view", new Guid("5b757ede-93e1-4c7a-88d3-5e89007d648b") });

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -32,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "office.users.manage", new Guid("5b757ede-93e1-4c7a-88d3-5e89007d648b") });

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -31,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "office.users.view", new Guid("5b757ede-93e1-4c7a-88d3-5e89007d648b") });

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -30,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "kawader.manage", new Guid("5b757ede-93e1-4c7a-88d3-5e89007d648b") });

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -29,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "nominations.manage", new Guid("5b757ede-93e1-4c7a-88d3-5e89007d648b") });

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -28,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "nominations.view", new Guid("5b757ede-93e1-4c7a-88d3-5e89007d648b") });

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -27,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "jobs.invitations.view", new Guid("5b757ede-93e1-4c7a-88d3-5e89007d648b") });

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -26,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "jobs.points.approve", new Guid("5b757ede-93e1-4c7a-88d3-5e89007d648b") });

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -25,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "jobs.points.view", new Guid("5b757ede-93e1-4c7a-88d3-5e89007d648b") });

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -24,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "jobs.points.manage", new Guid("5b757ede-93e1-4c7a-88d3-5e89007d648b") });

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -23,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "jobs.approve", new Guid("5b757ede-93e1-4c7a-88d3-5e89007d648b") });

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -22,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "jobs.manage", new Guid("5b757ede-93e1-4c7a-88d3-5e89007d648b") });

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -21,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "jobs.view", new Guid("5b757ede-93e1-4c7a-88d3-5e89007d648b") });

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -20,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "profile.approval.changes", new Guid("5b757ede-93e1-4c7a-88d3-5e89007d648b") });

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -19,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "profile.approval.review", new Guid("5b757ede-93e1-4c7a-88d3-5e89007d648b") });

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -18,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "profile.approval.view", new Guid("5b757ede-93e1-4c7a-88d3-5e89007d648b") });

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -17,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "profile.distribution.manage", new Guid("5b757ede-93e1-4c7a-88d3-5e89007d648b") });

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -16,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "profile.distribution.view", new Guid("5b757ede-93e1-4c7a-88d3-5e89007d648b") });

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -15,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "profile.manage", new Guid("5b757ede-93e1-4c7a-88d3-5e89007d648b") });

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -14,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "profile.view", new Guid("5b757ede-93e1-4c7a-88d3-5e89007d648b") });

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -13,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "roles.manage", new Guid("5b757ede-93e1-4c7a-88d3-5e89007d648b") });

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -12,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "roles.view", new Guid("5b757ede-93e1-4c7a-88d3-5e89007d648b") });

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -11,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "users.manage", new Guid("5b757ede-93e1-4c7a-88d3-5e89007d648b") });

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -10,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "users.view", new Guid("5b757ede-93e1-4c7a-88d3-5e89007d648b") });

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -9,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "dashboard.view", new Guid("5b757ede-93e1-4c7a-88d3-5e89007d648b") });

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -8,
                column: "ClaimValue",
                value: "profile.logs.view");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -7,
                column: "ClaimValue",
                value: "home.content.manage");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -6,
                column: "ClaimValue",
                value: "home.content.view");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -5,
                column: "ClaimValue",
                value: "roles.manage");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -4,
                column: "ClaimValue",
                value: "roles.view");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -3,
                column: "ClaimValue",
                value: "users.manage");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -2,
                column: "ClaimValue",
                value: "users.view");

            migrationBuilder.InsertData(
                table: "AspNetRoleClaims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "RoleId" },
                values: new object[,]
                {
                    { -84, "permission", "major-skill.management", new Guid("d8689e0c-d872-42e9-87b7-c3bb27305e07") },
                    { -83, "permission", "organization-structures.manage", new Guid("d8689e0c-d872-42e9-87b7-c3bb27305e07") }
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("26058720-5808-435a-abbf-d7a4e751f51e"),
                column: "UserTypeId",
                value: new Guid("a1b2c3d4-e5f6-4879-8a3b-5c6d7e8f9a0b"));

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("a8e0f354-2e23-41e1-9e1b-a1501b72dc4b"),
                column: "UserTypeId",
                value: new Guid("a1b2c3d4-e5f6-4879-8a3b-5c6d7e8f9a0b"));

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "Avatar", "ConcurrencyStamp", "CreatedById", "CreatedDate", "CurrentAuthToken", "DeletedById", "DeletedDate", "Discriminator", "Email", "EmailConfirmed", "EmployeeProfileId", "FullNameAr", "FullNameEn", "IsBlocked", "IsDeleted", "LastLoginDate", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "OtpAttempts", "OtpExpiry", "OtpLockedUntilUtc", "OtpReference", "OtpSendWindowStartUtc", "OtpSendsInWindow", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UpdatedById", "UpdatedDate", "UserName", "UserTypeId" },
                values: new object[,]
                {
                    { new Guid("207bd05b-ebd8-4cea-80ad-38fe2479eac6"), 0, null, "75a677a7-c93d-4940-8666-4d648343104c", null, new DateTime(1900, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, "EmployeeUser", "m.alhaddad@edu.gov.qa", true, null, "m.alhaddad", "m.alhaddad", false, false, null, false, null, "M.ALHADDAD@EDU.GOV.QA", "M.ALHADDAD@EDU.GOV.QA", 0, null, null, null, null, 0, null, null, false, "a984b6f5-e904-44b0-8d0d-5e93c07b1510", false, null, null, "m.alhaddad@edu.gov.qa", new Guid("a1b2c3d4-e5f6-4879-8a3b-5c6d7e8f9a0b") },
                    { new Guid("9fd109fb-a2ad-4637-86f2-2be13ccfa6d4"), 0, null, "75a677a7-c93d-4940-8666-4d648343104c", null, new DateTime(1900, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, "EmployeeUser", "na.almarri@edu.gov.qa", true, null, "na.almarri", "na.almarri", false, false, null, false, null, "NA.ALMARRI@EDU.GOV.QA", "NA.ALMARRI@EDU.GOV.QA", 0, null, null, null, null, 0, null, null, false, "a984b6f5-e904-44b0-8d0d-5e93c07b1510", false, null, null, "na.almarri@edu.gov.qa", new Guid("a1b2c3d4-e5f6-4879-8a3b-5c6d7e8f9a0b") }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { new Guid("5f12e420-f666-4af4-a8fa-4e4aa755fdcd"), new Guid("207bd05b-ebd8-4cea-80ad-38fe2479eac6") },
                    { new Guid("d8689e0c-d872-42e9-87b7-c3bb27305e07"), new Guid("207bd05b-ebd8-4cea-80ad-38fe2479eac6") },
                    { new Guid("5b757ede-93e1-4c7a-88d3-5e89007d648b"), new Guid("9fd109fb-a2ad-4637-86f2-2be13ccfa6d4") }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -84);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -83);

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("5f12e420-f666-4af4-a8fa-4e4aa755fdcd"), new Guid("207bd05b-ebd8-4cea-80ad-38fe2479eac6") });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("d8689e0c-d872-42e9-87b7-c3bb27305e07"), new Guid("207bd05b-ebd8-4cea-80ad-38fe2479eac6") });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("5b757ede-93e1-4c7a-88d3-5e89007d648b"), new Guid("9fd109fb-a2ad-4637-86f2-2be13ccfa6d4") });

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("207bd05b-ebd8-4cea-80ad-38fe2479eac6"));

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("9fd109fb-a2ad-4637-86f2-2be13ccfa6d4"));

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -82,
                column: "ClaimValue",
                value: "major-skill.management");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -81,
                column: "ClaimValue",
                value: "organization-structures.manage");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -80,
                column: "ClaimValue",
                value: "candidate.users.manage");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -79,
                column: "ClaimValue",
                value: "candidate.users.view");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -78,
                column: "ClaimValue",
                value: "office.users.manage");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -77,
                column: "ClaimValue",
                value: "office.users.view");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -76,
                column: "ClaimValue",
                value: "kawader.manage");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -75,
                column: "ClaimValue",
                value: "nominations.manage");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -74,
                column: "ClaimValue",
                value: "nominations.view");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -73,
                column: "ClaimValue",
                value: "jobs.invitations.view");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -72,
                column: "ClaimValue",
                value: "jobs.points.approve");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -71,
                column: "ClaimValue",
                value: "jobs.points.view");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -70,
                column: "ClaimValue",
                value: "jobs.points.manage");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -69,
                column: "ClaimValue",
                value: "jobs.approve");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -68,
                column: "ClaimValue",
                value: "jobs.manage");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -67,
                column: "ClaimValue",
                value: "jobs.view");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -66,
                column: "ClaimValue",
                value: "profile.approval.changes");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -65,
                column: "ClaimValue",
                value: "profile.approval.review");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -64,
                column: "ClaimValue",
                value: "profile.approval.view");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -63,
                column: "ClaimValue",
                value: "profile.distribution.manage");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -62,
                column: "ClaimValue",
                value: "profile.distribution.view");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -61,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "profile.manage", new Guid("d8689e0c-d872-42e9-87b7-c3bb27305e07") });

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -60,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "profile.view", new Guid("d8689e0c-d872-42e9-87b7-c3bb27305e07") });

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -59,
                column: "ClaimValue",
                value: "profile.approval.changes");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -58,
                column: "ClaimValue",
                value: "profile.approval.review");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -57,
                column: "ClaimValue",
                value: "profile.approval.view");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -56,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "profile.manage", new Guid("12f5805d-6970-4a9e-a275-2b7cf3db3bb8") });

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -55,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "profile.view", new Guid("12f5805d-6970-4a9e-a275-2b7cf3db3bb8") });

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -54,
                column: "ClaimValue",
                value: "profile.approval.changes");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -53,
                column: "ClaimValue",
                value: "profile.approval.review");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -52,
                column: "ClaimValue",
                value: "profile.approval.view");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -51,
                column: "ClaimValue",
                value: "profile.distribution.manage");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -50,
                column: "ClaimValue",
                value: "profile.distribution.view");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -49,
                column: "ClaimValue",
                value: "profile.manage");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -48,
                column: "ClaimValue",
                value: "profile.view");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -47,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "office.users.manage", new Guid("98e20970-b6bc-4da9-a947-f75e9adae3ca") });

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -46,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "office.users.view", new Guid("98e20970-b6bc-4da9-a947-f75e9adae3ca") });

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -45,
                column: "ClaimValue",
                value: "dashboard.view");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -44,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "dashboard.view", new Guid("5b757ede-93e1-4c7a-88d3-5e89007d648b") });

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -43,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "organization-structures.manage", new Guid("1361d691-53c5-4a84-aea1-64ff134cf082") });

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -42,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "major-skill.management", new Guid("1361d691-53c5-4a84-aea1-64ff134cf082") });

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -41,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "office.users.manage", new Guid("1361d691-53c5-4a84-aea1-64ff134cf082") });

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -40,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "office.users.view", new Guid("1361d691-53c5-4a84-aea1-64ff134cf082") });

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -39,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "kawader.manage", new Guid("1361d691-53c5-4a84-aea1-64ff134cf082") });

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -38,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "nominations.manage", new Guid("1361d691-53c5-4a84-aea1-64ff134cf082") });

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -37,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "nominations.view", new Guid("1361d691-53c5-4a84-aea1-64ff134cf082") });

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -36,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "jobs.invitations.view", new Guid("1361d691-53c5-4a84-aea1-64ff134cf082") });

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -35,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "jobs.points.approve", new Guid("1361d691-53c5-4a84-aea1-64ff134cf082") });

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -34,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "jobs.points.view", new Guid("1361d691-53c5-4a84-aea1-64ff134cf082") });

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -33,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "jobs.points.manage", new Guid("1361d691-53c5-4a84-aea1-64ff134cf082") });

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -32,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "jobs.approve", new Guid("1361d691-53c5-4a84-aea1-64ff134cf082") });

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -31,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "jobs.manage", new Guid("1361d691-53c5-4a84-aea1-64ff134cf082") });

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -30,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "jobs.view", new Guid("1361d691-53c5-4a84-aea1-64ff134cf082") });

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -29,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "profile.approval.changes", new Guid("1361d691-53c5-4a84-aea1-64ff134cf082") });

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -28,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "profile.approval.review", new Guid("1361d691-53c5-4a84-aea1-64ff134cf082") });

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -27,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "profile.approval.view", new Guid("1361d691-53c5-4a84-aea1-64ff134cf082") });

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -26,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "profile.distribution.manage", new Guid("1361d691-53c5-4a84-aea1-64ff134cf082") });

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -25,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "profile.distribution.view", new Guid("1361d691-53c5-4a84-aea1-64ff134cf082") });

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -24,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "profile.manage", new Guid("1361d691-53c5-4a84-aea1-64ff134cf082") });

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -23,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "profile.view", new Guid("1361d691-53c5-4a84-aea1-64ff134cf082") });

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -22,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "profile.logs.view", new Guid("1361d691-53c5-4a84-aea1-64ff134cf082") });

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -21,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "home.content.manage", new Guid("1361d691-53c5-4a84-aea1-64ff134cf082") });

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -20,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "home.content.view", new Guid("1361d691-53c5-4a84-aea1-64ff134cf082") });

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -19,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "targetentities.manage", new Guid("1361d691-53c5-4a84-aea1-64ff134cf082") });

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -18,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "targetentities.view", new Guid("1361d691-53c5-4a84-aea1-64ff134cf082") });

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -17,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "universities.manage", new Guid("1361d691-53c5-4a84-aea1-64ff134cf082") });

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -16,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "universities.view", new Guid("1361d691-53c5-4a84-aea1-64ff134cf082") });

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -15,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "countries.manage", new Guid("1361d691-53c5-4a84-aea1-64ff134cf082") });

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -14,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "countries.view", new Guid("1361d691-53c5-4a84-aea1-64ff134cf082") });

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -13,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "religions.manage", new Guid("1361d691-53c5-4a84-aea1-64ff134cf082") });

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -12,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "religions.view", new Guid("1361d691-53c5-4a84-aea1-64ff134cf082") });

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -11,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "languages.manage", new Guid("1361d691-53c5-4a84-aea1-64ff134cf082") });

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -10,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "languages.view", new Guid("1361d691-53c5-4a84-aea1-64ff134cf082") });

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -9,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "offices.manage", new Guid("1361d691-53c5-4a84-aea1-64ff134cf082") });

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -8,
                column: "ClaimValue",
                value: "offices.view");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -7,
                column: "ClaimValue",
                value: "candidate.users.manage");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -6,
                column: "ClaimValue",
                value: "candidate.users.view");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -5,
                column: "ClaimValue",
                value: "users.manage");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -4,
                column: "ClaimValue",
                value: "users.view");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -3,
                column: "ClaimValue",
                value: "roles.manage");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -2,
                column: "ClaimValue",
                value: "roles.view");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("26058720-5808-435a-abbf-d7a4e751f51e"),
                column: "UserTypeId",
                value: new Guid("c3d4e5f6-a7b8-6a95-0c3d-7e8f9a0b1c2d"));

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("a8e0f354-2e23-41e1-9e1b-a1501b72dc4b"),
                column: "UserTypeId",
                value: new Guid("c3d4e5f6-a7b8-6a95-0c3d-7e8f9a0b1c2d"));

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "UserType",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsActive", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[] { new Guid("c3d4e5f6-a7b8-6a95-0c3d-7e8f9a0b1c2d"), "Admin", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, 3, true, false, "مسؤول النظام", "Administrator", null, null });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "Avatar", "ConcurrencyStamp", "CreatedById", "CreatedDate", "CurrentAuthToken", "DeletedById", "DeletedDate", "Discriminator", "Email", "EmailConfirmed", "FullNameAr", "FullNameEn", "IsBlocked", "IsDeleted", "LastLoginDate", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "OtpAttempts", "OtpExpiry", "OtpLockedUntilUtc", "OtpReference", "OtpSendWindowStartUtc", "OtpSendsInWindow", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UpdatedById", "UpdatedDate", "UserName", "UserTypeId" },
                values: new object[] { new Guid("48f6d7a2-e821-4ab6-81cf-884ed650d2ec"), 0, null, "75a677a7-c93d-4940-8666-4d648343104c", null, new DateTime(1900, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, "AdminUser", "m.alhaddad@edu.gov.qa", true, "m.alhaddad", "m.alhaddad", false, false, null, false, null, "M.ALHADDAD@EDU.GOV.QA", "M.ALHADDAD@EDU.GOV.QA", 0, null, null, null, null, 0, null, null, false, "a984b6f5-e904-44b0-8d0d-5e93c07b1510", false, null, null, "m.alhaddad@edu.gov.qa", new Guid("c3d4e5f6-a7b8-6a95-0c3d-7e8f9a0b1c2d") });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { new Guid("1361d691-53c5-4a84-aea1-64ff134cf082"), new Guid("48f6d7a2-e821-4ab6-81cf-884ed650d2ec") });
        }
    }
}
