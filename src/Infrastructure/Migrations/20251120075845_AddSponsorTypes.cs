using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Tawtheef.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSponsorTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DisabilityDetails",
                schema: "app",
                table: "UserProfile",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HasDisability",
                schema: "app",
                table: "UserProfile",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "SponsorProfileId",
                schema: "app",
                table: "UserProfile",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SponsorType",
                schema: "lkp",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DeletedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    BackendName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DescriptionEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DescriptionAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SponsorType", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SponsorType_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SponsorType_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SponsorType_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SponsorProfile",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SponsorTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SponsorName = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: false),
                    SponsorNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SponsorCardId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DeletedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SponsorProfile", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SponsorProfile_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SponsorProfile_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SponsorProfile_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SponsorProfile_Resources_SponsorCardId",
                        column: x => x.SponsorCardId,
                        principalTable: "Resources",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SponsorProfile_SponsorType_SponsorTypeId",
                        column: x => x.SponsorTypeId,
                        principalSchema: "lkp",
                        principalTable: "SponsorType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "SponsorType",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("51588ec8-2d50-4365-aa8c-84efaec02e09"), "Company", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 2, false, "منشأة", "Company", null, null },
                    { new Guid("5970a291-e636-4fd4-ab27-2af22dd77e9a"), "Individual", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 1, false, "فرد", "Individual", null, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserProfile_SponsorProfileId",
                schema: "app",
                table: "UserProfile",
                column: "SponsorProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_SponsorProfile_CreatedById",
                schema: "app",
                table: "SponsorProfile",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_SponsorProfile_DeletedById",
                schema: "app",
                table: "SponsorProfile",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_SponsorProfile_SponsorCardId",
                schema: "app",
                table: "SponsorProfile",
                column: "SponsorCardId");

            migrationBuilder.CreateIndex(
                name: "IX_SponsorProfile_SponsorTypeId",
                schema: "app",
                table: "SponsorProfile",
                column: "SponsorTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_SponsorProfile_UpdatedById",
                schema: "app",
                table: "SponsorProfile",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_SponsorType_BackendName",
                schema: "lkp",
                table: "SponsorType",
                column: "BackendName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SponsorType_CreatedById",
                schema: "lkp",
                table: "SponsorType",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_SponsorType_DeletedById",
                schema: "lkp",
                table: "SponsorType",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_SponsorType_DisplayOrder",
                schema: "lkp",
                table: "SponsorType",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_SponsorType_UpdatedById",
                schema: "lkp",
                table: "SponsorType",
                column: "UpdatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfile_SponsorProfile_SponsorProfileId",
                schema: "app",
                table: "UserProfile",
                column: "SponsorProfileId",
                principalSchema: "app",
                principalTable: "SponsorProfile",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserProfile_SponsorProfile_SponsorProfileId",
                schema: "app",
                table: "UserProfile");

            migrationBuilder.DropTable(
                name: "SponsorProfile",
                schema: "app");

            migrationBuilder.DropTable(
                name: "SponsorType",
                schema: "lkp");

            migrationBuilder.DropIndex(
                name: "IX_UserProfile_SponsorProfileId",
                schema: "app",
                table: "UserProfile");

            migrationBuilder.DropColumn(
                name: "DisabilityDetails",
                schema: "app",
                table: "UserProfile");

            migrationBuilder.DropColumn(
                name: "HasDisability",
                schema: "app",
                table: "UserProfile");

            migrationBuilder.DropColumn(
                name: "SponsorProfileId",
                schema: "app",
                table: "UserProfile");
        }
    }
}
