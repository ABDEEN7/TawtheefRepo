using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tawtheef.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProfileAssigment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProfileAssignment",
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
                    UserProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    AssignedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UnassignedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfileAssignment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProfileAssignment_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProfileAssignment_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProfileAssignment_AspNetUsers_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProfileAssignment_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProfileAssignment_UserProfile_UserProfileId",
                        column: x => x.UserProfileId,
                        principalSchema: "app",
                        principalTable: "UserProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProfileAssignment_CreatedById",
                schema: "hr",
                table: "ProfileAssignment",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileAssignment_DeletedById",
                schema: "hr",
                table: "ProfileAssignment",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileAssignment_EmployeeId",
                schema: "hr",
                table: "ProfileAssignment",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileAssignment_UpdatedById",
                schema: "hr",
                table: "ProfileAssignment",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileAssignment_UserProfileId",
                schema: "hr",
                table: "ProfileAssignment",
                column: "UserProfileId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProfileAssignment",
                schema: "hr");
        }
    }
}
