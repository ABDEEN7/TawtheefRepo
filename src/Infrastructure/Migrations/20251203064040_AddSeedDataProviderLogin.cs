using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Tawtheef.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSeedDataProviderLogin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "NationalNumber",
                schema: "app",
                table: "UserProfile",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "ProviderLogin",
                schema: "lkp",
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
                    BackendName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DescriptionAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DescriptionEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProviderLogin", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProviderLogin_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProviderLogin_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProviderLogin_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CandidateTypeProviderLogin",
                schema: "lkp",
                columns: table => new
                {
                    CandidateTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProviderLoginId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CandidateTypeProviderLogin", x => new { x.CandidateTypeId, x.ProviderLoginId });
                    table.ForeignKey(
                        name: "FK_CandidateTypeProviderLogin_CandidateType_CandidateTypeId",
                        column: x => x.CandidateTypeId,
                        principalSchema: "lkp",
                        principalTable: "CandidateType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CandidateTypeProviderLogin_ProviderLogin_ProviderLoginId",
                        column: x => x.ProviderLoginId,
                        principalSchema: "lkp",
                        principalTable: "ProviderLogin",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "ProviderLogin",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("0d1ab8a4-2b89-4dcc-aa6f-6ec92ccb887c"), "Google", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 1, false, "جوجل", "Google", null, null },
                    { new Guid("b8854959-1e46-4595-b51f-de3c09e3ed85"), "QatarPass", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 2, false, "قطر باس", "QatarPass", null, null },
                    { new Guid("e0575116-ea2b-4917-965f-214048a4c78b"), "Azure", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 3, false, "أزور", "Azure", null, null }
                });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "CandidateTypeProviderLogin",
                columns: new[] { "CandidateTypeId", "ProviderLoginId" },
                values: new object[,]
                {
                    { new Guid("268d49f6-0dda-4dd6-8346-be355c553496"), new Guid("b8854959-1e46-4595-b51f-de3c09e3ed85") },
                    { new Guid("42b374d1-8032-44d7-95bd-ff5a64abbc95"), new Guid("0d1ab8a4-2b89-4dcc-aa6f-6ec92ccb887c") },
                    { new Guid("50f14c55-d5f0-4bba-930e-ab73b012e6cb"), new Guid("0d1ab8a4-2b89-4dcc-aa6f-6ec92ccb887c") },
                    { new Guid("744256ea-4ee0-4a63-b071-8810895a33dc"), new Guid("b8854959-1e46-4595-b51f-de3c09e3ed85") },
                    { new Guid("7b06bc91-88b3-46ec-b6cc-ef2338864d41"), new Guid("b8854959-1e46-4595-b51f-de3c09e3ed85") },
                    { new Guid("ce67465e-6fed-4a83-9161-8eba16a79af3"), new Guid("b8854959-1e46-4595-b51f-de3c09e3ed85") }
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserProfile_NationalNumber",
                schema: "app",
                table: "UserProfile",
                column: "NationalNumber",
                unique: true,
                filter: "[NationalNumber] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CandidateTypeProviderLogin_ProviderLoginId",
                schema: "lkp",
                table: "CandidateTypeProviderLogin",
                column: "ProviderLoginId");

            migrationBuilder.CreateIndex(
                name: "IX_ProviderLogin_BackendName",
                schema: "lkp",
                table: "ProviderLogin",
                column: "BackendName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProviderLogin_CreatedById",
                schema: "lkp",
                table: "ProviderLogin",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ProviderLogin_DeletedById",
                schema: "lkp",
                table: "ProviderLogin",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_ProviderLogin_DisplayOrder",
                schema: "lkp",
                table: "ProviderLogin",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_ProviderLogin_UpdatedById",
                schema: "lkp",
                table: "ProviderLogin",
                column: "UpdatedById");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CandidateTypeProviderLogin",
                schema: "lkp");

            migrationBuilder.DropTable(
                name: "ProviderLogin",
                schema: "lkp");

            migrationBuilder.DropIndex(
                name: "IX_UserProfile_NationalNumber",
                schema: "app",
                table: "UserProfile");

            migrationBuilder.AlterColumn<string>(
                name: "NationalNumber",
                schema: "app",
                table: "UserProfile",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);
        }
    }
}
