using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tawtheef.Infrastructure.Migrations.Development
{
    /// <inheritdoc />
    public partial class Check : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("12f5805d-6970-4a9e-a275-2b7cf3db3bb8"),
                column: "ConcurrencyStamp",
                value: "db98add5-8e90-47db-96db-f291baa280ed");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("1361d691-53c5-4a84-aea1-64ff134cf082"),
                column: "ConcurrencyStamp",
                value: "e01c6405-9903-4c59-a0ca-ac117ce2a93b");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("5f12e420-f666-4af4-a8fa-4e4aa755fdcd"),
                column: "ConcurrencyStamp",
                value: "06d0913c-071b-46ef-a4e5-185cf422d706");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("98e20970-b6bc-4da9-a947-f75e9adae3ca"),
                column: "ConcurrencyStamp",
                value: "dd7c8bfb-1aeb-426b-a51f-5affe6bd1b96");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
        }
    }
}
