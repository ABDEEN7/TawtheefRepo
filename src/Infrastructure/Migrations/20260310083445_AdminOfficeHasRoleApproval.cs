using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Tawtheef.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AdminOfficeHasRoleApproval : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("1361d691-53c5-4a84-aea1-64ff134cf082"), new Guid("5a4c8063-07a4-43ba-b4a8-8d980a323738") });

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("5a4c8063-07a4-43ba-b4a8-8d980a323738"));

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
                column: "ClaimValue",
                value: "profile.manage");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -60,
                column: "ClaimValue",
                value: "profile.view");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -59,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "profile.approval.changes", new Guid("12f5805d-6970-4a9e-a275-2b7cf3db3bb8") });

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -58,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "profile.approval.review", new Guid("12f5805d-6970-4a9e-a275-2b7cf3db3bb8") });

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -57,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "profile.approval.view", new Guid("12f5805d-6970-4a9e-a275-2b7cf3db3bb8") });

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -56,
                column: "ClaimValue",
                value: "profile.manage");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -55,
                column: "ClaimValue",
                value: "profile.view");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -54,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "profile.approval.changes", new Guid("98e20970-b6bc-4da9-a947-f75e9adae3ca") });

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -53,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "profile.approval.review", new Guid("98e20970-b6bc-4da9-a947-f75e9adae3ca") });

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -52,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "profile.approval.view", new Guid("98e20970-b6bc-4da9-a947-f75e9adae3ca") });

            migrationBuilder.InsertData(
                table: "AspNetRoleClaims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "RoleId" },
                values: new object[,]
                {
                    { -82, "permission", "major-skill.management", new Guid("d8689e0c-d872-42e9-87b7-c3bb27305e07") },
                    { -81, "permission", "organization-structures.manage", new Guid("d8689e0c-d872-42e9-87b7-c3bb27305e07") },
                    { -80, "permission", "candidate.users.manage", new Guid("d8689e0c-d872-42e9-87b7-c3bb27305e07") }
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("48f6d7a2-e821-4ab6-81cf-884ed650d2ec"),
                columns: new[] { "Email", "FullNameAr", "FullNameEn", "NormalizedEmail", "NormalizedUserName", "UserName" },
                values: new object[] { "m.alhaddad@edu.gov.qa", "m.alhaddad", "m.alhaddad", "M.ALHADDAD@EDU.GOV.QA", "M.ALHADDAD@EDU.GOV.QA", "m.alhaddad@edu.gov.qa" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -82);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -81);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -80);

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -79,
                column: "ClaimValue",
                value: "major-skill.management");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -78,
                column: "ClaimValue",
                value: "organization-structures.manage");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -77,
                column: "ClaimValue",
                value: "candidate.users.manage");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -76,
                column: "ClaimValue",
                value: "candidate.users.view");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -75,
                column: "ClaimValue",
                value: "office.users.manage");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -74,
                column: "ClaimValue",
                value: "office.users.view");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -73,
                column: "ClaimValue",
                value: "kawader.manage");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -72,
                column: "ClaimValue",
                value: "nominations.manage");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -71,
                column: "ClaimValue",
                value: "nominations.view");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -70,
                column: "ClaimValue",
                value: "jobs.invitations.view");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -69,
                column: "ClaimValue",
                value: "jobs.points.approve");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -68,
                column: "ClaimValue",
                value: "jobs.points.view");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -67,
                column: "ClaimValue",
                value: "jobs.points.manage");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -66,
                column: "ClaimValue",
                value: "jobs.approve");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -65,
                column: "ClaimValue",
                value: "jobs.manage");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -64,
                column: "ClaimValue",
                value: "jobs.view");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -63,
                column: "ClaimValue",
                value: "profile.approval.changes");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -62,
                column: "ClaimValue",
                value: "profile.approval.review");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -61,
                column: "ClaimValue",
                value: "profile.approval.view");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -60,
                column: "ClaimValue",
                value: "profile.distribution.manage");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -59,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "profile.distribution.view", new Guid("d8689e0c-d872-42e9-87b7-c3bb27305e07") });

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -58,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "profile.manage", new Guid("d8689e0c-d872-42e9-87b7-c3bb27305e07") });

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -57,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "profile.view", new Guid("d8689e0c-d872-42e9-87b7-c3bb27305e07") });

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -56,
                column: "ClaimValue",
                value: "profile.approval.changes");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -55,
                column: "ClaimValue",
                value: "profile.approval.review");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -54,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "profile.approval.view", new Guid("12f5805d-6970-4a9e-a275-2b7cf3db3bb8") });

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -53,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "profile.manage", new Guid("12f5805d-6970-4a9e-a275-2b7cf3db3bb8") });

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -52,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "profile.view", new Guid("12f5805d-6970-4a9e-a275-2b7cf3db3bb8") });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("48f6d7a2-e821-4ab6-81cf-884ed650d2ec"),
                columns: new[] { "Email", "FullNameAr", "FullNameEn", "NormalizedEmail", "NormalizedUserName", "UserName" },
                values: new object[] { "t-m.khatatbeh-dev@edu.gov.qa", "t-m.khatatbeh-dev", "t-m.khatatbeh-dev", "T-M.KHATATBEH-DEV@EDU.GOV.QA", "T-M.KHATATBEH-DEV@EDU.GOV.QA", "t-m.khatatbeh-dev@edu.gov.qa" });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "Avatar", "ConcurrencyStamp", "CreatedById", "CreatedDate", "CurrentAuthToken", "DeletedById", "DeletedDate", "Discriminator", "Email", "EmailConfirmed", "FullNameAr", "FullNameEn", "IsBlocked", "IsDeleted", "LastLoginDate", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "OtpAttempts", "OtpExpiry", "OtpLockedUntilUtc", "OtpReference", "OtpSendWindowStartUtc", "OtpSendsInWindow", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UpdatedById", "UpdatedDate", "UserName", "UserTypeId" },
                values: new object[] { new Guid("5a4c8063-07a4-43ba-b4a8-8d980a323738"), 0, null, "75a677a7-c93d-4940-8666-4d648343104c", null, new DateTime(1900, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, "AdminUser", "t-hu.ahmed-dev@edu.gov.qa", true, "t-hu.ahmed-dev", "t-hu.ahmed-dev", false, false, null, false, null, "T-HU.AHMED-DEV@EDU.GOV.QA", "T-HU.AHMED-DEV@EDU.GOV.QA", 0, null, null, null, null, 0, null, null, false, "a984b6f5-e904-44b0-8d0d-5e93c07b1510", false, null, null, "t-hu.ahmed-dev@edu.gov.qa", new Guid("c3d4e5f6-a7b8-6a95-0c3d-7e8f9a0b1c2d") });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { new Guid("1361d691-53c5-4a84-aea1-64ff134cf082"), new Guid("5a4c8063-07a4-43ba-b4a8-8d980a323738") });
        }
    }
}
