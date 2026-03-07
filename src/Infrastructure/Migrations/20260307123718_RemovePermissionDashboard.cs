using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Tawtheef.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemovePermissionDashboard : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -83);

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
                column: "ClaimValue",
                value: "profile.approval.view");

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
                column: "ClaimValue",
                value: "office.users.manage");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -46,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "office.users.view", new Guid("98e20970-b6bc-4da9-a947-f75e9adae3ca") });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -79,
                column: "ClaimValue",
                value: "office.users.manage");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -78,
                column: "ClaimValue",
                value: "office.users.view");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -77,
                column: "ClaimValue",
                value: "kawader.manage");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -76,
                column: "ClaimValue",
                value: "nominations.manage");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -75,
                column: "ClaimValue",
                value: "nominations.view");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -74,
                column: "ClaimValue",
                value: "jobs.invitations.view");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -73,
                column: "ClaimValue",
                value: "jobs.points.approve");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -72,
                column: "ClaimValue",
                value: "jobs.points.view");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -71,
                column: "ClaimValue",
                value: "jobs.points.manage");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -70,
                column: "ClaimValue",
                value: "jobs.approve");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -69,
                column: "ClaimValue",
                value: "jobs.manage");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -68,
                column: "ClaimValue",
                value: "jobs.view");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -67,
                column: "ClaimValue",
                value: "profile.approval.changes");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -66,
                column: "ClaimValue",
                value: "profile.approval.review");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -65,
                column: "ClaimValue",
                value: "profile.approval.view");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -64,
                column: "ClaimValue",
                value: "profile.distribution.manage");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -63,
                column: "ClaimValue",
                value: "profile.distribution.view");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -62,
                column: "ClaimValue",
                value: "profile.manage");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -61,
                column: "ClaimValue",
                value: "profile.view");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -60,
                column: "ClaimValue",
                value: "dashboard.view");

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
                column: "ClaimValue",
                value: "dashboard.view");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -53,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "profile.distribution.manage", new Guid("98e20970-b6bc-4da9-a947-f75e9adae3ca") });

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -52,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "profile.distribution.view", new Guid("98e20970-b6bc-4da9-a947-f75e9adae3ca") });

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
                column: "ClaimValue",
                value: "dashboard.view");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: -46,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "dashboard.view", new Guid("5f12e420-f666-4af4-a8fa-4e4aa755fdcd") });

            migrationBuilder.InsertData(
                table: "AspNetRoleClaims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "RoleId" },
                values: new object[,]
                {
                    { -83, "permission", "major-skill.management", new Guid("d8689e0c-d872-42e9-87b7-c3bb27305e07") },
                    { -82, "permission", "organization-structures.manage", new Guid("d8689e0c-d872-42e9-87b7-c3bb27305e07") },
                    { -81, "permission", "candidate.users.manage", new Guid("d8689e0c-d872-42e9-87b7-c3bb27305e07") },
                    { -80, "permission", "candidate.users.view", new Guid("d8689e0c-d872-42e9-87b7-c3bb27305e07") }
                });
        }
    }
}
