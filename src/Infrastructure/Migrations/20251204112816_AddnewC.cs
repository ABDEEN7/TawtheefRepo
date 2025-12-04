using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tawtheef.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddnewC : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("2464b38a-5439-421d-ac39-e0bfbdcbc17e"),
                column: "Status",
                value: "InCreation");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("a2c6d553-1c3d-4697-99d2-7e12f6335f38"),
                column: "Status",
                value: "InCreation");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("bf879671-62de-4f17-8c07-38b6e6619fe0"),
                column: "Status",
                value: "InCreation");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("2464b38a-5439-421d-ac39-e0bfbdcbc17e"),
                column: "Status",
                value: null);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("a2c6d553-1c3d-4697-99d2-7e12f6335f38"),
                column: "Status",
                value: null);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("bf879671-62de-4f17-8c07-38b6e6619fe0"),
                column: "Status",
                value: null);
        }
    }
}
