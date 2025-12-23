using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Tawtheef.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "pro");

            migrationBuilder.EnsureSchema(
                name: "lkp");

            migrationBuilder.EnsureSchema(
                name: "hr");

            migrationBuilder.EnsureSchema(
                name: "app");

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NameEn = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DescriptionAr = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    DescriptionEn = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Achievement",
                schema: "pro",
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
                    AchievementTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IssuingAuthority = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CountryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IssueDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RelatedToSpecialization = table.Column<bool>(type: "bit", nullable: true),
                    AttachmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Achievement", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AchievementType",
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
                    table.PrimaryKey("PK_AchievementType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FullNameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FullNameAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsBlocked = table.Column<bool>(type: "bit", nullable: false),
                    LastLoginDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Avatar = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    UserTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CurrentAuthToken = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    OtpReference = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: true),
                    OtpExpiry = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OtpAttempts = table.Column<int>(type: "int", nullable: false),
                    OtpSends = table.Column<int>(type: "int", nullable: false),
                    Discriminator = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AuditTrailEntry",
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
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActionType = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    Section = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    EntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AttachmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditTrailEntry", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuditTrailEntry_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AuditTrailEntry_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AuditTrailEntry_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CandidateType",
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
                    table.PrimaryKey("PK_CandidateType", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CandidateType_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CandidateType_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CandidateType_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ContactVerification",
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
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Destination = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    ExpiresAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UsedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactVerification", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContactVerification_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ContactVerification_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ContactVerification_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ContactVerification_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Country",
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
                    Code = table.Column<int>(type: "int", nullable: false),
                    ISOCode = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    CodeAlpha = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    BackendName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DescriptionAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DescriptionEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Country", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Country_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Country_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Country_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Degree",
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
                    table.PrimaryKey("PK_Degree", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Degree_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Degree_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Degree_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmailQueues",
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
                    RecipientEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Body = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsSent = table.Column<bool>(type: "bit", nullable: false),
                    SentDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    RetryCount = table.Column<int>(type: "int", nullable: false),
                    ErrorMessage = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailQueues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmailQueues_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmailQueues_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmailQueues_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmailTemplates",
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
                    TemplateKey = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BodyTemplate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailTemplates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmailTemplates_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmailTemplates_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmailTemplates_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EntityLog",
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
                    EntityType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ActionDetails = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChangeDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OldValues = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NewValues = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IPAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChangedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityLog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EntityLog_AspNetUsers_ChangedByUserId",
                        column: x => x.ChangedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EntityLog_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EntityLog_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EntityLog_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Gender",
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
                    table.PrimaryKey("PK_Gender", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Gender_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Gender_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Gender_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InvitationStatus",
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
                    table.PrimaryKey("PK_InvitationStatus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvitationStatus_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InvitationStatus_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InvitationStatus_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "JobCategory",
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
                    table.PrimaryKey("PK_JobCategory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobCategory_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobCategory_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobCategory_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "JobStatus",
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
                    table.PrimaryKey("PK_JobStatus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobStatus_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobStatus_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobStatus_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Language",
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
                    table.PrimaryKey("PK_Language", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Language_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Language_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Language_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LanguageLevel",
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
                    table.PrimaryKey("PK_LanguageLevel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LanguageLevel_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LanguageLevel_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LanguageLevel_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Major",
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
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    BackendName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DescriptionAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DescriptionEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Major", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Major_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Major_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Major_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Major_Major_ParentId",
                        column: x => x.ParentId,
                        principalSchema: "lkp",
                        principalTable: "Major",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MaritalStatus",
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
                    table.PrimaryKey("PK_MaritalStatus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaritalStatus_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MaritalStatus_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MaritalStatus_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
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
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ToAddress = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Channel = table.Column<int>(type: "int", nullable: false),
                    TemplateKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Body = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PayloadJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProviderMessageId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Error = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    SentAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifications_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Notifications_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Notifications_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Notifications_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ProfileSubmission",
                schema: "pro",
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
                    Version = table.Column<int>(type: "int", nullable: false),
                    SubmittedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SnapshotJson = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfileSubmission", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProfileSubmission_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProfileSubmission_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProfileSubmission_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

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
                name: "RatingGrade",
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
                    table.PrimaryKey("PK_RatingGrade", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RatingGrade_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RatingGrade_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RatingGrade_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RefreshToken",
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
                    Token = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Expires = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedByIp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RevokedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    RevokedByIp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReplacedByToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RevokedReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserDeviceId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshToken", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshToken_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RefreshToken_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RefreshToken_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RefreshToken_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Religion",
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
                    table.PrimaryKey("PK_Religion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Religion_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Religion_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Religion_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Resources",
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
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Url = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false),
                    Key = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Size = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    AdditionalData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContentHash = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Resources", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Resources_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Resources_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Resources_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Sector",
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
                    table.PrimaryKey("PK_Sector", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sector_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Sector_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Sector_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SkillLevel",
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
                    table.PrimaryKey("PK_SkillLevel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SkillLevel_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SkillLevel_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SkillLevel_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SkillRequirementType",
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
                    table.PrimaryKey("PK_SkillRequirementType", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SkillRequirementType_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SkillRequirementType_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SkillRequirementType_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SkillType",
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
                    table.PrimaryKey("PK_SkillType", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SkillType_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SkillType_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SkillType_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SponsorType",
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
                name: "StudyType",
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
                    table.PrimaryKey("PK_StudyType", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudyType_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudyType_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudyType_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TargetEntity",
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
                    table.PrimaryKey("PK_TargetEntity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TargetEntity_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TargetEntity_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TargetEntity_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserSession",
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
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SessionId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RevokedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Ip = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Platform = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AppVersion = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSession", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserSession_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserSession_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserSession_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserSession_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserType",
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
                    table.PrimaryKey("PK_UserType", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserType_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserType_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserType_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WorkType",
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
                    table.PrimaryKey("PK_WorkType", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkType_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkType_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkType_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "City",
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
                    CountryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BackendName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DescriptionAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DescriptionEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_City", x => x.Id);
                    table.ForeignKey(
                        name: "FK_City_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_City_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_City_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_City_Country_CountryId",
                        column: x => x.CountryId,
                        principalSchema: "lkp",
                        principalTable: "Country",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Office",
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
                    CountryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    BackendName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DescriptionAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DescriptionEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Office", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Office_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Office_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Office_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Office_Country_CountryId",
                        column: x => x.CountryId,
                        principalSchema: "lkp",
                        principalTable: "Country",
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
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CandidateTypeProviderLogin_ProviderLogin_ProviderLoginId",
                        column: x => x.ProviderLoginId,
                        principalSchema: "lkp",
                        principalTable: "ProviderLogin",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ResidenceAddress",
                schema: "pro",
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
                    BuildingNo = table.Column<int>(type: "int", nullable: false),
                    StreetNo = table.Column<int>(type: "int", nullable: false),
                    ZoneNo = table.Column<int>(type: "int", nullable: false),
                    UnitNo = table.Column<int>(type: "int", nullable: false),
                    CertificateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResidenceAddress", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResidenceAddress_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ResidenceAddress_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ResidenceAddress_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ResidenceAddress_Resources_CertificateId",
                        column: x => x.CertificateId,
                        principalTable: "Resources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Management",
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
                    SectorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BackendName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DescriptionAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DescriptionEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Management", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Management_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Management_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Management_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Management_Sector_SectorId",
                        column: x => x.SectorId,
                        principalSchema: "lkp",
                        principalTable: "Sector",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Skill",
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
                    MajorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SkillTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SkillRequirementTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    BackendName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DescriptionAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DescriptionEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Skill", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Skill_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Skill_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Skill_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Skill_Major_MajorId",
                        column: x => x.MajorId,
                        principalSchema: "lkp",
                        principalTable: "Major",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Skill_SkillRequirementType_SkillRequirementTypeId",
                        column: x => x.SkillRequirementTypeId,
                        principalSchema: "lkp",
                        principalTable: "SkillRequirementType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Skill_SkillType_SkillTypeId",
                        column: x => x.SkillTypeId,
                        principalSchema: "lkp",
                        principalTable: "SkillType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SponsorProfile",
                schema: "app",
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
                    SponsorTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SponsorName = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: false),
                    SponsorNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    QIDExpiry = table.Column<DateOnly>(type: "date", nullable: false),
                    SponsorCardId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
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

            migrationBuilder.CreateTable(
                name: "LoginAttempt",
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
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Source = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Succeeded = table.Column<bool>(type: "bit", nullable: false),
                    FailureReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SessionId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AttemptedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoginAttempt", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LoginAttempt_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LoginAttempt_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LoginAttempt_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LoginAttempt_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LoginAttempt_UserType_UserTypeId",
                        column: x => x.UserTypeId,
                        principalSchema: "lkp",
                        principalTable: "UserType",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "University",
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
                    CityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WebSite = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LogoEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LogoAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OriginalName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BackendName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DescriptionAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DescriptionEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_University", x => x.Id);
                    table.ForeignKey(
                        name: "FK_University_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_University_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_University_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_University_City_CityId",
                        column: x => x.CityId,
                        principalSchema: "lkp",
                        principalTable: "City",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Department",
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
                    ManagementId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BackendName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DescriptionAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DescriptionEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Department", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Department_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Department_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Department_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Department_Management_ManagementId",
                        column: x => x.ManagementId,
                        principalSchema: "lkp",
                        principalTable: "Management",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserProfile",
                schema: "app",
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
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CandidateTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TargetEntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OfficeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ResumeAttachmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    NationalCardId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    NationalNumber = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    QIDExpiry = table.Column<DateOnly>(type: "date", nullable: true),
                    BirthDate = table.Column<DateOnly>(type: "date", nullable: true),
                    NationalityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    GenderId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReligionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    MaritalStatusId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ChildrenCount = table.Column<int>(type: "int", nullable: false),
                    ResidenceCountryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    InterviewLocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResidenceAddressId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    HasDisability = table.Column<bool>(type: "bit", nullable: false),
                    DisabilityDetails = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SponsorProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    BirthdayCertificateId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    MarriageCertificateId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "InCreation")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProfile", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserProfile_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserProfile_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserProfile_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserProfile_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserProfile_CandidateType_CandidateTypeId",
                        column: x => x.CandidateTypeId,
                        principalSchema: "lkp",
                        principalTable: "CandidateType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserProfile_Country_InterviewLocationId",
                        column: x => x.InterviewLocationId,
                        principalSchema: "lkp",
                        principalTable: "Country",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserProfile_Country_NationalityId",
                        column: x => x.NationalityId,
                        principalSchema: "lkp",
                        principalTable: "Country",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserProfile_Country_ResidenceCountryId",
                        column: x => x.ResidenceCountryId,
                        principalSchema: "lkp",
                        principalTable: "Country",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserProfile_Gender_GenderId",
                        column: x => x.GenderId,
                        principalSchema: "lkp",
                        principalTable: "Gender",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserProfile_MaritalStatus_MaritalStatusId",
                        column: x => x.MaritalStatusId,
                        principalSchema: "lkp",
                        principalTable: "MaritalStatus",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserProfile_Office_OfficeId",
                        column: x => x.OfficeId,
                        principalSchema: "lkp",
                        principalTable: "Office",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserProfile_Religion_ReligionId",
                        column: x => x.ReligionId,
                        principalSchema: "lkp",
                        principalTable: "Religion",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserProfile_ResidenceAddress_ResidenceAddressId",
                        column: x => x.ResidenceAddressId,
                        principalSchema: "pro",
                        principalTable: "ResidenceAddress",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserProfile_Resources_BirthdayCertificateId",
                        column: x => x.BirthdayCertificateId,
                        principalTable: "Resources",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserProfile_Resources_MarriageCertificateId",
                        column: x => x.MarriageCertificateId,
                        principalTable: "Resources",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserProfile_Resources_NationalCardId",
                        column: x => x.NationalCardId,
                        principalTable: "Resources",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserProfile_Resources_ResumeAttachmentId",
                        column: x => x.ResumeAttachmentId,
                        principalTable: "Resources",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserProfile_SponsorProfile_SponsorProfileId",
                        column: x => x.SponsorProfileId,
                        principalSchema: "app",
                        principalTable: "SponsorProfile",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserProfile_TargetEntity_TargetEntityId",
                        column: x => x.TargetEntityId,
                        principalSchema: "lkp",
                        principalTable: "TargetEntity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Job",
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
                    TitleAr = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    TitleEn = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    SectorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ManagementId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DepartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    YearsOfExperience = table.Column<int>(type: "int", nullable: false),
                    JobCategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WorkLocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GenderId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    MajorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubMajorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    WorkTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NumberOfVacancies = table.Column<int>(type: "int", nullable: false),
                    ClosingDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    PublishAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CancelledAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    MinimumAge = table.Column<int>(type: "int", nullable: false),
                    MaximumAge = table.Column<int>(type: "int", nullable: false),
                    JobStatusId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OverViewAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OverViewEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BenefitsAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BenefitsEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QualificationDescriptionAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QualificationDescriptionEn = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Job", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Job_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Job_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Job_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Job_Department_DepartmentId",
                        column: x => x.DepartmentId,
                        principalSchema: "lkp",
                        principalTable: "Department",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Job_Gender_GenderId",
                        column: x => x.GenderId,
                        principalSchema: "lkp",
                        principalTable: "Gender",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Job_JobCategory_JobCategoryId",
                        column: x => x.JobCategoryId,
                        principalSchema: "lkp",
                        principalTable: "JobCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Job_JobStatus_JobStatusId",
                        column: x => x.JobStatusId,
                        principalSchema: "lkp",
                        principalTable: "JobStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Job_Major_MajorId",
                        column: x => x.MajorId,
                        principalSchema: "lkp",
                        principalTable: "Major",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Job_Major_SubMajorId",
                        column: x => x.SubMajorId,
                        principalSchema: "lkp",
                        principalTable: "Major",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Job_Management_ManagementId",
                        column: x => x.ManagementId,
                        principalSchema: "lkp",
                        principalTable: "Management",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Job_Sector_SectorId",
                        column: x => x.SectorId,
                        principalSchema: "lkp",
                        principalTable: "Sector",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Job_TargetEntity_WorkLocationId",
                        column: x => x.WorkLocationId,
                        principalSchema: "lkp",
                        principalTable: "TargetEntity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Job_WorkType_WorkTypeId",
                        column: x => x.WorkTypeId,
                        principalSchema: "lkp",
                        principalTable: "WorkType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProfileAdditionalAttachment",
                schema: "pro",
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
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AttachmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfileAdditionalAttachment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProfileAdditionalAttachment_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProfileAdditionalAttachment_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProfileAdditionalAttachment_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProfileAdditionalAttachment_Resources_AttachmentId",
                        column: x => x.AttachmentId,
                        principalTable: "Resources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProfileAdditionalAttachment_UserProfile_UserProfileId",
                        column: x => x.UserProfileId,
                        principalSchema: "app",
                        principalTable: "UserProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateTable(
                name: "ProfileChange",
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
                    Section = table.Column<int>(type: "int", nullable: false),
                    TargetType = table.Column<int>(type: "int", nullable: false),
                    FieldPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EntityName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ResourceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AttachmentTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OldValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NewValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfileChange", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProfileChange_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProfileChange_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProfileChange_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProfileChange_UserProfile_UserProfileId",
                        column: x => x.UserProfileId,
                        principalSchema: "app",
                        principalTable: "UserProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProfileLanguage",
                schema: "pro",
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
                    LanguageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SpeakingLevelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WritingLevelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReadingLevelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfileLanguage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProfileLanguage_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProfileLanguage_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProfileLanguage_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProfileLanguage_LanguageLevel_ReadingLevelId",
                        column: x => x.ReadingLevelId,
                        principalSchema: "lkp",
                        principalTable: "LanguageLevel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProfileLanguage_LanguageLevel_SpeakingLevelId",
                        column: x => x.SpeakingLevelId,
                        principalSchema: "lkp",
                        principalTable: "LanguageLevel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProfileLanguage_LanguageLevel_WritingLevelId",
                        column: x => x.WritingLevelId,
                        principalSchema: "lkp",
                        principalTable: "LanguageLevel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProfileLanguage_Language_LanguageId",
                        column: x => x.LanguageId,
                        principalSchema: "lkp",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProfileLanguage_UserProfile_UserProfileId",
                        column: x => x.UserProfileId,
                        principalSchema: "app",
                        principalTable: "UserProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProfileReviewDecision",
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
                    Action = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Summary = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AttachmentResourceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ExceptionalFlag = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfileReviewDecision", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProfileReviewDecision_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProfileReviewDecision_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProfileReviewDecision_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProfileReviewDecision_Resources_AttachmentResourceId",
                        column: x => x.AttachmentResourceId,
                        principalTable: "Resources",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProfileReviewDecision_UserProfile_UserProfileId",
                        column: x => x.UserProfileId,
                        principalSchema: "app",
                        principalTable: "UserProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProfileSkill",
                schema: "pro",
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
                    SkillId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LevelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfileSkill", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProfileSkill_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProfileSkill_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProfileSkill_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProfileSkill_SkillLevel_LevelId",
                        column: x => x.LevelId,
                        principalSchema: "lkp",
                        principalTable: "SkillLevel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProfileSkill_SkillType_SkillId",
                        column: x => x.SkillId,
                        principalSchema: "lkp",
                        principalTable: "SkillType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProfileSkill_UserProfile_UserProfileId",
                        column: x => x.UserProfileId,
                        principalSchema: "app",
                        principalTable: "UserProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Qualification",
                schema: "pro",
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
                    DegreeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CountryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MajorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SubMajorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UniversityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    GraduationYear = table.Column<int>(type: "int", nullable: true),
                    StudyTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    GPA = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    RatingId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CertificateId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Qualification", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Qualification_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Qualification_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Qualification_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Qualification_Country_CountryId",
                        column: x => x.CountryId,
                        principalSchema: "lkp",
                        principalTable: "Country",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Qualification_Degree_DegreeId",
                        column: x => x.DegreeId,
                        principalSchema: "lkp",
                        principalTable: "Degree",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Qualification_Major_MajorId",
                        column: x => x.MajorId,
                        principalSchema: "lkp",
                        principalTable: "Major",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Qualification_Major_SubMajorId",
                        column: x => x.SubMajorId,
                        principalSchema: "lkp",
                        principalTable: "Major",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Qualification_RatingGrade_RatingId",
                        column: x => x.RatingId,
                        principalSchema: "lkp",
                        principalTable: "RatingGrade",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Qualification_Resources_CertificateId",
                        column: x => x.CertificateId,
                        principalTable: "Resources",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Qualification_StudyType_StudyTypeId",
                        column: x => x.StudyTypeId,
                        principalSchema: "lkp",
                        principalTable: "StudyType",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Qualification_University_UniversityId",
                        column: x => x.UniversityId,
                        principalSchema: "lkp",
                        principalTable: "University",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Qualification_UserProfile_UserProfileId",
                        column: x => x.UserProfileId,
                        principalSchema: "app",
                        principalTable: "UserProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TrainingCourse",
                schema: "pro",
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
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Provider = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CountryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SpecializationRelation = table.Column<int>(type: "int", nullable: true),
                    CertificateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingCourse", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrainingCourse_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TrainingCourse_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TrainingCourse_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TrainingCourse_Country_CountryId",
                        column: x => x.CountryId,
                        principalSchema: "lkp",
                        principalTable: "Country",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TrainingCourse_Resources_CertificateId",
                        column: x => x.CertificateId,
                        principalTable: "Resources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TrainingCourse_UserProfile_UserProfileId",
                        column: x => x.UserProfileId,
                        principalSchema: "app",
                        principalTable: "UserProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Invitations",
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
                    JobId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsAccepted = table.Column<bool>(type: "bit", nullable: false),
                    AcceptedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    InvitationStatusId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invitations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Invitations_AspNetUsers_ApplicantId",
                        column: x => x.ApplicantId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Invitations_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Invitations_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Invitations_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Invitations_InvitationStatus_InvitationStatusId",
                        column: x => x.InvitationStatusId,
                        principalSchema: "lkp",
                        principalTable: "InvitationStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Invitations_Job_JobId",
                        column: x => x.JobId,
                        principalSchema: "hr",
                        principalTable: "Job",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "JobCondition",
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
                    JobId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TextAr = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TextEn = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobCondition", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobCondition_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobCondition_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobCondition_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobCondition_Job_JobId",
                        column: x => x.JobId,
                        principalSchema: "hr",
                        principalTable: "Job",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "JobDegree",
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
                    JobId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DegreeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobDegree", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobDegree_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobDegree_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobDegree_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobDegree_Degree_DegreeId",
                        column: x => x.DegreeId,
                        principalSchema: "lkp",
                        principalTable: "Degree",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JobDegree_Job_JobId",
                        column: x => x.JobId,
                        principalSchema: "hr",
                        principalTable: "Job",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "JobRequiredAttachment",
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
                    JobId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TitleAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TitleEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsMandatory = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobRequiredAttachment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobRequiredAttachment_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobRequiredAttachment_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobRequiredAttachment_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobRequiredAttachment_Job_JobId",
                        column: x => x.JobId,
                        principalSchema: "hr",
                        principalTable: "Job",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "JobResponsibility",
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
                    JobId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TextAr = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TextEn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsMandatory = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobResponsibility", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobResponsibility_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobResponsibility_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobResponsibility_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobResponsibility_Job_JobId",
                        column: x => x.JobId,
                        principalSchema: "hr",
                        principalTable: "Job",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "JobSkill",
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
                    JobId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SkillId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShowToApplicants = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobSkill", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobSkill_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobSkill_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobSkill_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobSkill_Job_JobId",
                        column: x => x.JobId,
                        principalSchema: "hr",
                        principalTable: "Job",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobSkill_Skill_SkillId",
                        column: x => x.SkillId,
                        principalSchema: "lkp",
                        principalTable: "Skill",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "JobTabReviewNote",
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
                    JobId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Tab = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    TabStatus = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    IsResolved = table.Column<bool>(type: "bit", nullable: false),
                    ReviewCycleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobTabReviewNote", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobTabReviewNote_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobTabReviewNote_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobTabReviewNote_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobTabReviewNote_Job_JobId",
                        column: x => x.JobId,
                        principalSchema: "hr",
                        principalTable: "Job",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReviewItem",
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
                    ProfileChangeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false),
                    TargetType = table.Column<int>(type: "int", nullable: false),
                    Section = table.Column<int>(type: "int", nullable: false),
                    FieldPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EntityName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ResourceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AttachmentTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ReviewedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReviewedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewerNote = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApprovedHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CurrentHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsOutdated = table.Column<bool>(type: "bit", nullable: false),
                    ApprovedAtVersion = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReviewItem_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReviewItem_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReviewItem_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReviewItem_ProfileChange_ProfileChangeId",
                        column: x => x.ProfileChangeId,
                        principalSchema: "hr",
                        principalTable: "ProfileChange",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ReviewItem_UserProfile_UserProfileId",
                        column: x => x.UserProfileId,
                        principalSchema: "app",
                        principalTable: "UserProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Experience",
                schema: "pro",
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
                    EmployerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    JobTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CountryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SpecializationRelation = table.Column<int>(type: "int", nullable: true),
                    QualificationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CertificateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Experience", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Experience_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Experience_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Experience_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Experience_Country_CountryId",
                        column: x => x.CountryId,
                        principalSchema: "lkp",
                        principalTable: "Country",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Experience_Qualification_QualificationId",
                        column: x => x.QualificationId,
                        principalSchema: "pro",
                        principalTable: "Qualification",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Experience_Resources_CertificateId",
                        column: x => x.CertificateId,
                        principalTable: "Resources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Experience_UserProfile_UserProfileId",
                        column: x => x.UserProfileId,
                        principalSchema: "app",
                        principalTable: "UserProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HistoryInvitation",
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
                    InvitationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InvitationStatusId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoryInvitation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HistoryInvitation_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HistoryInvitation_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HistoryInvitation_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HistoryInvitation_InvitationStatus_InvitationStatusId",
                        column: x => x.InvitationStatusId,
                        principalSchema: "lkp",
                        principalTable: "InvitationStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HistoryInvitation_Invitations_InvitationId",
                        column: x => x.InvitationId,
                        principalTable: "Invitations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JobTabReviewAttachment",
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
                    JobTabReviewNoteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AttachmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobTabReviewAttachment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobTabReviewAttachment_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobTabReviewAttachment_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobTabReviewAttachment_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobTabReviewAttachment_JobTabReviewNote_JobTabReviewNoteId",
                        column: x => x.JobTabReviewNoteId,
                        principalSchema: "hr",
                        principalTable: "JobTabReviewNote",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobTabReviewAttachment_Resources_AttachmentId",
                        column: x => x.AttachmentId,
                        principalTable: "Resources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "AchievementType",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("3f5f4d1f-b214-44cb-9b9e-2f9ec3c1f7f1"), "Certificate", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "شهادة", "Certificate", 1, false, "شهادة", "Certificate", null, null },
                    { new Guid("6c3b1b1b-0c9c-4e0e-8d1e-4d6c12e91533"), "Award", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "جائزة", "Award", 2, false, "جائزة", "Award", null, null }
                });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "CandidateType",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("268d49f6-0dda-4dd6-8346-be355c553496"), "Qatari", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "قطري الجنسية", "Qatari National", 1, false, "قطري", "Qatari", null, null },
                    { new Guid("42b374d1-8032-44d7-95bd-ff5a64abbc95"), "NonQatari", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "مقيم خارج دولة قطر", "Resident outside Qatar", 4, false, "مقيم خارج قطر", "Resident outside Qatar", null, null },
                    { new Guid("50f14c55-d5f0-4bba-930e-ab73b012e6cb"), "GCC", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "مواطن دول مجلس التعاون الخليجي", "Citizen of a GCC country", 2, false, "مواطن دول مجلس التعاون الخليجي", "Citizen of a GCC country", null, null },
                    { new Guid("744256ea-4ee0-4a63-b071-8810895a33dc"), "SonOfQatariMother", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "أبناء المرأة القطرية المتزوجة من غير قطري", "Children of a Qatari woman married to a non-Qatari", 5, false, "أبناء المرأة القطرية المتزوجة من غير قطري", "Children of a Qatari woman married to a non-Qatari", null, null },
                    { new Guid("7b06bc91-88b3-46ec-b6cc-ef2338864d41"), "ResidentQatar", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "مقيم داخل دولة قطر", "Resident in Qatar", 3, false, "مقيم في قطر", "Resident in Qatar", null, null },
                    { new Guid("ce67465e-6fed-4a83-9161-8eba16a79af3"), "WifeOfQatari", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "الزوج أو الزوجة غير قطري/ة المتزوج/ة من قطري/ة", "Non-Qatari spouse married to a Qatari", 6, false, "الزوج أو الزوجة غير قطري/ة المتزوج/ة من قطري/ة", "Non-Qatari spouse married to a Qatari", null, null }
                });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "Degree",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("1af8c855-975f-4a49-bcf0-343a6d7fd8df"), "PostgraduateDiploma", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "دبلوم دراسات عليا", "Postgraduate Diploma", 0, false, "دبلوم دراسات عليا", "Postgraduate Diploma", null, null },
                    { new Guid("6e453f48-5f2f-4f98-8b76-f416cdd4811b"), "Primary", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "ابتدائي", "Primary", 0, false, "ابتدائي", "Primary", null, null },
                    { new Guid("8ca14478-019d-4d3e-89cd-90cb89baf463"), "Master", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "ماجستير", "Master", 0, false, "ماجستير", "Master's", null, null },
                    { new Guid("d60cb9b1-f0ce-4c0a-a147-51e8d3947ef4"), "Doctorate", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "دكتوراه", "Doctorate", 0, false, "دكتوراه", "PhD", null, null },
                    { new Guid("de5901db-60dd-49f0-953c-daf923cf9f4a"), "Bachelor", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "بكالوريوس", "Bachelor", 0, false, "بكالوريوس", "Bachelor's", null, null },
                    { new Guid("ebf2faa1-5ce6-4a04-9472-2746bfbbd252"), "Secondary", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "ثانوي", "Secondary", 0, false, "ثانوي", "Secondary", null, null },
                    { new Guid("f1a31fe6-ba80-46cb-b24b-f402bcb4fdec"), "Preparatory", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "إعدادي", "Preparatory", 0, false, "إعدادي", "Preparatory", null, null },
                    { new Guid("f6249ce2-fa02-4e18-9c89-151ba3be0c12"), "IntermediateDiploma", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "دبلوم متوسط", "Intermediate Diploma", 0, false, "دبلوم متوسط", "Intermediate Diploma", null, null }
                });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "Gender",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("03cbe4e3-dc47-4d0c-8a07-87917af1d2dd"), "Female", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "أنثى", "Female", 0, false, "أنثى", "Female", null, null },
                    { new Guid("4436a8ce-3f1e-4581-be3d-839c67127a9f"), "All", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "جميع الخيارات", "All Options", 0, false, "جميعها", "All", null, null },
                    { new Guid("6720e352-b360-41f0-8d3b-fa5116b7a0b4"), "Male", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "ذكر", "Male", 0, false, "ذكر", "Male", null, null }
                });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "InvitationStatus",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("22ef7e86-28cb-4a30-98bc-7d45f9b44de3"), "Approved", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "تمت الموافقة على الطلب وانتقل لمرحلة لاحقة.", "The application has been approved and moved to a later stage.", 6, false, "معتمد", "Approved", null, null },
                    { new Guid("43ad4950-46a9-4b37-b4e2-4a2d8ac4a7c4"), "UnderReview", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "الطلب تحت المراجعة المبدئية لقسم التوظيف.", "The application is under initial review by the recruitment department.", 4, false, "قيد المراجعة", "Under Review", null, null },
                    { new Guid("5602dbce-00a3-488b-9156-981a0fb18a01"), "RequiresUpdate", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "الطلب تم إرجاعه للمرشح لإكمال نواقص محددة.", "The application was returned to the candidate to complete missing information.", 5, false, "مطلوب تعديل", "Requires Update", null, null },
                    { new Guid("64236c6a-167a-4213-b1d6-80c2c8c86dde"), "Readed", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "المرشح فتح الدعوة لأول مرة ولم يبدأ التقديم.", "The candidate opened the invitation for the first time but has not started the application.", 2, false, "تمت القراءة", "Read", null, null },
                    { new Guid("7103ba49-ad43-4751-b1a5-9084aca69676"), "Rejected", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "الطلب لم يتم قبوله لأسباب وظيفية أو تنظيمية.", "The application was not accepted for functional or organizational reasons.", 7, false, "مرفوض", "Rejected", null, null },
                    { new Guid("bced01d8-3784-4e2c-9312-930951cc59d8"), "Cancelled", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "المرشح قام بإلغاء الطلب أو تم إلغاؤه وفق الإجراءات.", "The candidate cancelled the application or it was cancelled procedurally.", 8, false, "ملغي", "Cancelled", null, null },
                    { new Guid("ca054592-8617-406d-8e8f-3a773b3d0d5e"), "Closed", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "الوظيفة انتهت أو أُغلقت من قبل الموارد البشرية ولا يمكن اتخاذ أي إجراء عليها.", "The job has ended or was closed by HR and no further action can be taken.", 9, false, "مغلق", "Closed", null, null },
                    { new Guid("e7b28f10-fb23-466e-a9b2-aaa3c9afdeac"), "Submitted", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "المرشح قدم طلبه وجميع بياناته مكتملة.", "The candidate submitted the application with all required information completed.", 3, false, "تم التقديم", "Submitted", null, null },
                    { new Guid("f0bc801d-f54c-4a0e-8aae-00694e4fc80d"), "NewInvitation", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "وظيفة تمت دعوة المرشح لها ولم يقم بقراءتها أو فتحها بعد.", "The candidate was invited but has not opened or viewed it yet.", 1, false, "دعوة جديدة", "New Invitation", null, null }
                });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "JobCategory",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("3d31e01b-9c57-4b47-8584-9fa8949838e8"), "Labor", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "فئة الوظائف العمالية", "Category for labor jobs", 0, false, "عمالي", "Labor", null, null },
                    { new Guid("4e7c8fe7-475b-4e4d-99f0-cfef85020d5b"), "Administrative", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "فئة الوظائف الإدارية", "Category for administrative jobs", 0, false, "إداري", "Administrative", null, null },
                    { new Guid("62ecfbcc-7ae7-45c0-9db4-fd50751a336e"), "Academic", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "فئة الوظائف الأكاديمية", "Category for academic jobs", 0, false, "أكاديمي", "Academic", null, null }
                });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "JobStatus",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("0d21e063-48d3-d320-4078-d85a7c2bf622"), "ReadyForAnnouncement", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "الوظيفة جاهزة للإعلان.", "Job is ready to be announced.", 7, false, "جاهزة للإعلان", "Ready For Announcement", null, null },
                    { new Guid("114dae76-bde9-3efa-2a2b-803ebd92e109"), "Closed", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "تم الوصول إلى تاريخ نهاية التقديم ولن يُسمح بإرسال الطلبات.", "Application end date has been reached; job is not accepting new applications.", 3, false, "متوقفة", "Closed", null, null },
                    { new Guid("1e3ecad5-63fa-a11c-7acb-dd4c62ef74fd"), "Published", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "تم نشر الوظيفة.", "Job is published.", 8, false, "منشورة", "Published", null, null },
                    { new Guid("5c360b07-157c-630a-254a-9c01587d80a8"), "Approved", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "تم اعتماد الوظيفة.", "Job was approved.", 6, false, "معتمدة", "Approved", null, null },
                    { new Guid("c0d95787-8505-b84f-6f50-0b461ece500d"), "Cancelled", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "تم إلغاء الوظيفة ككل ولن يتم نشرها على المنصة.", "The job has been cancelled and will not be published on the platform.", 4, false, "ملغية", "Cancelled", null, null },
                    { new Guid("c64916a6-4bd2-a4b6-afbe-c5c3b4926530"), "Draft", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "تم إنشاء الوظيفة كمسودة ولم يتم إرسالها للجمهور.", "Job is created as a draft and not visible to the public.", 1, false, "مسودة", "Draft", null, null },
                    { new Guid("e07bd71f-c466-0eea-49cc-2b13d1d9403f"), "PendingApproval", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "الوظيفة قيد الأعتماد.", "Job is waiting for approval.", 5, false, "قيد الأعتماد", "Pending Approval", null, null },
                    { new Guid("e0cd7b22-8948-0c37-9b15-2e5217f00065"), "NeedUpdate", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "الوظيفة تحتاج للتعديل", "Job needs to be updated.", 1, false, "تحتاج للتعديل", "Need Update", null, null },
                    { new Guid("e0cd7b22-8948-0c37-9b15-2e5217f0c565"), "Active", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "الوظيفة جاهزة للتقديم ويمكن إرسالها للجمهور المطلوب.", "Job is open for applications and can be published to the target audience.", 2, false, "نشطة", "Active", null, null },
                    { new Guid("f2e7748a-12fe-53ac-fbdf-e989f8aa498a"), "Rejected", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "تم رفض الوظيفة.", "Job was rejected.", 9, false, "مرفوضة", "Rejected", null, null }
                });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "Language",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("8672c4c2-f635-4d26-843a-223fba3a6322"), "English", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 2, false, "الإنجليزية", "English", null, null },
                    { new Guid("9843b692-ef7c-44a7-b1e8-1dcf2b6d87dd"), "Arabic", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 1, false, "العربية", "Arabic", null, null }
                });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "LanguageLevel",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("5161f23a-f501-414c-b4fe-81cde9ebcd5d"), "Expert", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 4, false, "خبير", "Expert", null, null },
                    { new Guid("9aa2c1bc-2040-40e1-86a2-72cac90928e1"), "Advanced", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 3, false, "متقدم", "Advanced", null, null },
                    { new Guid("afd6d7b4-9e20-40bc-a571-0df1670761b0"), "Intermediate", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 2, false, "متوسط", "Intermediate", null, null },
                    { new Guid("c7d8b159-c9dc-48a1-be6d-26493423f8a9"), "Native", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 4, false, "لغة أم", "Native", null, null },
                    { new Guid("f0c9e84a-258f-4f6b-a469-153a92d68730"), "Basic", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 1, false, "مبتدئ", "Basic", null, null }
                });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "MaritalStatus",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("18e83653-978c-44c8-8691-43f95d9a5b7d"), "Divorced", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 3, false, "مطلق", "Divorced", null, null },
                    { new Guid("7113eb36-44b8-457a-96e9-61bfe6a06f05"), "Married", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 2, false, "متزوج", "Married", null, null },
                    { new Guid("8f22e74f-672b-47f4-8f32-93f9dbcb15da"), "Widowed", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 4, false, "أرمل", "Widowed", null, null },
                    { new Guid("c28ab4a0-59b0-4a64-82c4-ba1bd89efaba"), "Single", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 1, false, "أعزب", "Single", null, null }
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
                table: "RatingGrade",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("08e6f782-458e-4334-9bf1-f599c53b437a"), "Acceptable", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 5, false, "مقبول", "Acceptable", null, null },
                    { new Guid("2cf3d4e2-33cd-4671-9667-1b5e6e0cee9a"), "VeryGood", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 3, false, "جيد جدا", "Very Good", null, null },
                    { new Guid("4dbd3381-f57a-4f6c-a718-432970d32276"), "AboveExcellent", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 1, false, "امتياز", "Above Excellent", null, null },
                    { new Guid("71a78988-825a-4a0f-94a3-f59089dbe33d"), "Good", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 4, false, "جيد", "Good", null, null },
                    { new Guid("76bd9c52-9c81-4d8f-afb4-fdd924744082"), "Excellent", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 2, false, "ممتاز", "Excellent", null, null }
                });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "Religion",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("185cbc18-2a59-40b0-a8d9-64b451f91ddf"), "Other", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 6, false, "أخرى", "Other", null, null },
                    { new Guid("51588ec8-2d50-4365-aa8c-84efaec02e09"), "Christian", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 2, false, "المسيحية", "Christianity", null, null },
                    { new Guid("5970a291-e636-4fd4-ab27-2af22dd77e9a"), "Islam", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 1, false, "الإسلام", "Islam", null, null },
                    { new Guid("6af60f76-d289-42d1-a3de-a3fa89056678"), "Sikh", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 5, false, "السيخية", "Sikhism", null, null },
                    { new Guid("7d76cd4c-915a-4ce2-a2b0-decddf0f7490"), "Hindu", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 3, false, "الهندوسية", "Hinduism", null, null },
                    { new Guid("c419dbdf-d0fc-4555-af2a-c5ed46847f8f"), "Buddhist", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 4, false, "البوذية", "Buddhism", null, null }
                });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "Sector",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("a3c9f0eb-5d6e-6c4f-0a7b-8c9d0e1a2b3c"), "PrivateEducationSector", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "قطاع التعليم الخاص", "Private Education Sector", 5, false, "التعليم الخاص", "Private Education Sector", null, null },
                    { new Guid("b4da01fc-6e7f-7d50-1b8c-9d0e1a2b3c4d"), "AssessmentSector", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "قطاع التقييم", "Assessment Sector", 6, false, "التقييم", "Assessment Sector", null, null },
                    { new Guid("c5eb12fd-7f80-8e61-2c9d-0e1a2b3c4d5e"), "SharedServicesSector", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "قطاع الخدمات المشتركة", "Shared Services Sector", 7, false, "الخدمات المشتركة", "Shared Services Sector", null, null },
                    { new Guid("e1a7f8d9-3b4c-4a2d-8e5f-6a7b8c9d0e1f"), "DeputyMinisterSector", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "قطاع سعادة الوكيل", "Deputy Minister Sector", 3, false, "سعادة الوكيل", "Deputy Minister Sector", null, null },
                    { new Guid("f2b8e9fa-4c5d-5b3e-9f6a-7b8c9d0e1a2b"), "GeneralEducationSector", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "قطاع التعليم العام", "General Education Sector", 4, false, "التعليم العام", "General Education Sector", null, null }
                });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "SkillLevel",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("2cf3d4e2-33cd-4671-9667-1b5e6e0cee9a"), "Intermediate", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 3, false, "متوسط", "Intermediate", null, null },
                    { new Guid("4dbd3381-f57a-4f6c-a718-432970d32276"), "Expert", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 1, false, "خبير", "Expert", null, null },
                    { new Guid("69c0943a-f04f-4b6c-9145-1fbfae4b5c2e"), "Basic", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 4, false, "أساسي", "Basic", null, null },
                    { new Guid("b8f10518-bf02-4357-a0e3-1f6bfb1746cd"), "Advanced", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 2, false, "متقدم", "Advanced", null, null }
                });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "SkillRequirementType",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("a014fa55-b3ed-14e4-b4aa-15354a5d1cc3"), "Optional", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "متطلب مهارة اختياري", "Optional skill requirement", 2, false, "اختياري", "Optional", null, null },
                    { new Guid("b0f1f4dd-95ad-3d0b-e74c-684d2d288329"), "Essential", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "متطلب مهارة أساسي", "Essential skill requirement", 1, false, "أساسي", "Essential", null, null }
                });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "SkillType",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("2f1b6ce7-cbc3-2b5c-b264-a02c6a87be1d"), "Educational", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "ãåÇÑÇÊ ÊÚáíãíÉ Êã ÇáÍÕæá ÚáíåÇ ãä ÎáÇá ÇáÊÚáíã ÇáÑÓãí", "Educational skills acquired through formal education", 1, false, "ÊÚáíãí", "Educational", null, null },
                    { new Guid("83b504d0-25c1-5ca0-6757-df299869f002"), "Professional", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "ãåÇÑÇÊ ãåäíÉ äÇÚãÉ æßÝÇÁÇÊ ãßÇä ÇáÚãá", "Professional soft skills and workplace competencies", 3, false, "ãåäí", "Professional", null, null },
                    { new Guid("d14ac141-c057-16f5-ddd8-97d02f6a7c9b"), "Technical", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "ãåÇÑÇÊ ÊÞäíÉ Ãæ ÕáÈÉ ÊÊÚáÞ ÈÃÏæÇÊ Ãæ ÊÞäíÇÊ Ãæ ãäåÌíÇÊ ãÍÏÏÉ", "Technical or hard skills related to specific tools, technologies, or methodologies", 2, false, "ÊÞäí", "Technical", null, null },
                    { new Guid("f91eb9e6-7a3f-76d6-1fe1-4443cecc5b9a"), "Other", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "ÃäæÇÚ ÃÎÑì ãä ÇáãåÇÑÇÊ ÛíÑ ÇáãÕäÝÉ ÃÚáÇå", "Other types of skills not categorized above", 4, false, "ÃÎÑì", "Other", null, null }
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

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "StudyType",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("0a09774f-fa61-4896-b808-c8576cd0bf6b"), "DistanceLearning", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 2, false, "تعليم عن بعد", "Distance Learning", null, null },
                    { new Guid("29754e1d-9125-4582-a998-68a72b8f8443"), "Affiliation", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 3, false, "دراسة انتساب", "Affiliation Study", null, null },
                    { new Guid("b28f6a3a-e3ec-401e-9bc7-c4156b4bf76f"), "Regular", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 1, false, "دراسة نظامية", "Regular Study", null, null }
                });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "TargetEntity",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("1548322c-6c05-4754-a89c-19e9d2443d62"), "Schools", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 1, false, "المدارس", "Schools", null, null },
                    { new Guid("8fadf8df-ae7e-4d22-8cb8-f79ed9b14375"), "Ministry", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 2, false, "الوزارة", "Ministry", null, null }
                });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "UserType",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("a1b2c3d4-e5f6-4879-8a3b-5c6d7e8f9a0b"), "Employee", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 1, false, "موظف", "Employee", null, null },
                    { new Guid("b2c3d4e5-f6a7-5984-9b2c-6d7e8f9a0b1c"), "Applicant", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 2, false, "متقدم", "Applicant", null, null },
                    { new Guid("c3d4e5f6-a7b8-6a95-0c3d-7e8f9a0b1c2d"), "Admin", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 3, false, "مسؤول النظام", "Administrator", null, null }
                });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "WorkType",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("0fdfadbf-e5a0-41d3-a74d-d86354fed434"), "FullTime", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "وظيفة بدوام كامل", "Full-time job", 1, false, "دوام كامل", "Full-Time", null, null },
                    { new Guid("e30074cd-52c6-41b5-a692-57626d109c73"), "PartTime", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "وظيفة بدوام جزئي", "Part-time job", 2, false, "دوام جزئي", "Part-Time", null, null }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "Avatar", "ConcurrencyStamp", "CreatedById", "CreatedDate", "CurrentAuthToken", "DeletedById", "DeletedDate", "Discriminator", "Email", "EmailConfirmed", "FullNameAr", "FullNameEn", "IsBlocked", "IsDeleted", "LastLoginDate", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "OtpAttempts", "OtpExpiry", "OtpReference", "OtpSends", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UpdatedById", "UpdatedDate", "UserName", "UserTypeId" },
                values: new object[,]
                {
                    { new Guid("2464b38a-5439-421d-ac39-e0bfbdcbc17e"), 0, null, "ae0628d3-8171-4a35-b168-fee762654e7a", null, new DateTimeOffset(new DateTime(1900, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 3, 0, 0, 0)), null, null, null, "EmployeeUser", "qa.e@tawtheef.com", true, "QA. E", "QA. E", false, false, null, false, null, "QA.E@TAWTHEEF.COM", "QA.E@TAWTHEEF.COM", 0, null, null, 0, "AQAAAAIAAYagAAAAEGcM+djZ3c2Q/N1kjZpDwcwjH2sfsRHkNS5H4lObrCaoiN238MHwgDFbLXiNsm5J4A==", null, false, "55669db7-ef3a-493f-89a6-a1d608403f83", false, null, null, "qa.e@tawtheef.com", new Guid("a1b2c3d4-e5f6-4879-8a3b-5c6d7e8f9a0b") },
                    { new Guid("a2c6d553-1c3d-4697-99d2-7e12f6335f38"), 0, null, "75a677a7-c93d-4940-8666-4d648343104c", null, new DateTimeOffset(new DateTime(1900, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 3, 0, 0, 0)), null, null, null, "AdminUser", "admin@tawtheef.com", true, "Admin", "Admin", false, false, null, false, null, "ADMIN@TAWTHEEF.COM", "ADMIN@TAWTHEEF.COM", 0, null, null, 0, "AQAAAAIAAYagAAAAEIgyaMwrHltJioEPUc/0/KfGb2iA529lr04a/6+LmeVbtJZ1fV3px6oeRlalT1GI/Q==", null, false, "a984b6f5-e904-44b0-8d0d-5e93c07b1510", false, null, null, "admin@tawtheef.com", new Guid("c3d4e5f6-a7b8-6a95-0c3d-7e8f9a0b1c2d") },
                    { new Guid("bf879671-62de-4f17-8c07-38b6e6619fe0"), 0, null, "af24df87-26d1-4f94-b84b-410bbbe14859", null, new DateTimeOffset(new DateTime(1900, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 3, 0, 0, 0)), null, null, null, "ApplicantUser", "qa.a@tawtheef.com", true, "QA. A", "QA. A", false, false, null, false, null, "QA.A@TAWTHEEF.COM", "QA.A@TAWTHEEF.COM", 0, null, null, 0, "AQAAAAIAAYagAAAAEN8QCL2z2kO862Y8bQpTxN7RPyssbCDnnWOBERadOw0vaVF5WAL3D/axQfbD1BgXKA==", null, false, "18cc15bc-1783-40d9-a3b5-d26dd57c3f6c", false, null, null, "qa.a@tawtheef.com", new Guid("b2c3d4e5-f6a7-5984-9b2c-6d7e8f9a0b1c") }
                });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "CandidateTypeProviderLogin",
                columns: new[] { "CandidateTypeId", "ProviderLoginId" },
                values: new object[,]
                {
                    { new Guid("42b374d1-8032-44d7-95bd-ff5a64abbc95"), new Guid("0d1ab8a4-2b89-4dcc-aa6f-6ec92ccb887c") },
                    { new Guid("50f14c55-d5f0-4bba-930e-ab73b012e6cb"), new Guid("0d1ab8a4-2b89-4dcc-aa6f-6ec92ccb887c") },
                    { new Guid("744256ea-4ee0-4a63-b071-8810895a33dc"), new Guid("b8854959-1e46-4595-b51f-de3c09e3ed85") },
                    { new Guid("7b06bc91-88b3-46ec-b6cc-ef2338864d41"), new Guid("b8854959-1e46-4595-b51f-de3c09e3ed85") },
                    { new Guid("ce67465e-6fed-4a83-9161-8eba16a79af3"), new Guid("b8854959-1e46-4595-b51f-de3c09e3ed85") }
                });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "Management",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsDeleted", "NameAr", "NameEn", "SectorId", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("453aa49d-8f16-a85b-9991-c8355ac8bf00"), "TrainingCenter", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 2, false, "مركز التدريب", "Training Center", new Guid("f2b8e9fa-4c5d-5b3e-9f6a-7b8c9d0e1a2b"), null, null },
                    { new Guid("4575068a-4f0d-b6cd-8ea8-be680d8dc992"), "Minister", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 1, false, "الوزير", "Minister", new Guid("e1a7f8d9-3b4c-4a2d-8e5f-6a7b8c9d0e1f"), null, null }
                });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "Department",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsDeleted", "ManagementId", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("0bfb044d-9f85-4f46-a1eb-920a1f9f519a"), "EarlyChildhoodEducation", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "تعليم الطفولة المبكرة", "Early Childhood Education Department", 6, false, new Guid("453aa49d-8f16-a85b-9991-c8355ac8bf00"), "تعليم الطفولة المبكرة", "Early Childhood Education", null, null },
                    { new Guid("1b01eb93-b6e3-447a-8d1c-a9ce59bf5ca7"), "HigherEducation", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "التعليم العالي", "Higher Education Department", 7, false, new Guid("453aa49d-8f16-a85b-9991-c8355ac8bf00"), "التعليم العالي", "Higher Education", null, null },
                    { new Guid("2a65e4a6-ca1b-4da3-8375-299ec39a50f9"), "PrimaryEducation", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "التعليم الابتدائي", "Primary Education Department", 8, false, new Guid("453aa49d-8f16-a85b-9991-c8355ac8bf00"), "التعليم الابتدائي", "Primary Education", null, null },
                    { new Guid("48d2a180-30dc-4314-b222-92750a9d0afe"), "InformationSystems", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "نظام الاعتماد", "Information Systems Department", 1, false, new Guid("4575068a-4f0d-b6cd-8ea8-be680d8dc992"), "نظام الاعتماد", "Information Systems", null, null },
                    { new Guid("6881d9fd-7281-457a-a2e1-3cbe827622e8"), "Evaluation", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "التقويم", "Evaluation Department", 4, false, new Guid("453aa49d-8f16-a85b-9991-c8355ac8bf00"), "التقويم", "Evaluation", null, null },
                    { new Guid("7e225d86-c5e5-4225-b9fc-d255a975f5d1"), "HumanResources", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "الموارد البشرية", "Human Resources Department", 2, false, new Guid("453aa49d-8f16-a85b-9991-c8355ac8bf00"), "الموارد البشرية", "Human Resources", null, null },
                    { new Guid("8acd1c68-9b65-40e1-8776-ce6d7afcf542"), "CommunicationsMedia", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "الاتصالات والاعلام", "Communications and Media Department", 10, false, new Guid("453aa49d-8f16-a85b-9991-c8355ac8bf00"), "الاتصالات والاعلام", "Communications and Media", null, null },
                    { new Guid("8fd80cb6-794a-4211-b8a4-139e8e026e5b"), "AdministrativeFinancialAffairs", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "الشؤون الادارية والمالية", "Administrative and Financial Affairs Department", 3, false, new Guid("453aa49d-8f16-a85b-9991-c8355ac8bf00"), "الشؤون الادارية والمالية", "Administrative and Financial Affairs", null, null },
                    { new Guid("91a511fb-9b43-47fd-9cd5-fc2a9ecbc32e"), "SchoolAffairs", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "شؤون المدارس", "School Affairs Department", 9, false, new Guid("453aa49d-8f16-a85b-9991-c8355ac8bf00"), "شؤون المدارس", "School Affairs", null, null },
                    { new Guid("b858ce40-a3ac-4d27-aba9-86ef62fab4fe"), "Curriculum", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "المناهج", "Curriculum Department", 5, false, new Guid("453aa49d-8f16-a85b-9991-c8355ac8bf00"), "المناهج", "Curriculum", null, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Achievement_AchievementTypeId",
                schema: "pro",
                table: "Achievement",
                column: "AchievementTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Achievement_AttachmentId",
                schema: "pro",
                table: "Achievement",
                column: "AttachmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Achievement_CountryId",
                schema: "pro",
                table: "Achievement",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Achievement_CreatedById",
                schema: "pro",
                table: "Achievement",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Achievement_DeletedById",
                schema: "pro",
                table: "Achievement",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_Achievement_UpdatedById",
                schema: "pro",
                table: "Achievement",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Achievement_UserProfileId",
                schema: "pro",
                table: "Achievement",
                column: "UserProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_AchievementType_BackendName",
                schema: "lkp",
                table: "AchievementType",
                column: "BackendName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AchievementType_CreatedById",
                schema: "lkp",
                table: "AchievementType",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_AchievementType_DeletedById",
                schema: "lkp",
                table: "AchievementType",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_AchievementType_DisplayOrder",
                schema: "lkp",
                table: "AchievementType",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_AchievementType_UpdatedById",
                schema: "lkp",
                table: "AchievementType",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_Email",
                table: "AspNetUsers",
                column: "Email",
                unique: true,
                filter: "[Email] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_UserTypeId",
                table: "AspNetUsers",
                column: "UserTypeId");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AuditTrailEntry_CreatedById",
                schema: "hr",
                table: "AuditTrailEntry",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_AuditTrailEntry_DeletedById",
                schema: "hr",
                table: "AuditTrailEntry",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_AuditTrailEntry_UpdatedById",
                schema: "hr",
                table: "AuditTrailEntry",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_CandidateType_BackendName",
                schema: "lkp",
                table: "CandidateType",
                column: "BackendName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CandidateType_CreatedById",
                schema: "lkp",
                table: "CandidateType",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_CandidateType_DeletedById",
                schema: "lkp",
                table: "CandidateType",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_CandidateType_DisplayOrder",
                schema: "lkp",
                table: "CandidateType",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_CandidateType_UpdatedById",
                schema: "lkp",
                table: "CandidateType",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_CandidateTypeProviderLogin_ProviderLoginId",
                schema: "lkp",
                table: "CandidateTypeProviderLogin",
                column: "ProviderLoginId");

            migrationBuilder.CreateIndex(
                name: "IX_City_BackendName",
                schema: "lkp",
                table: "City",
                column: "BackendName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_City_CountryId",
                schema: "lkp",
                table: "City",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_City_CreatedById",
                schema: "lkp",
                table: "City",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_City_DeletedById",
                schema: "lkp",
                table: "City",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_City_DisplayOrder",
                schema: "lkp",
                table: "City",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_City_UpdatedById",
                schema: "lkp",
                table: "City",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ContactVerification_CreatedById",
                table: "ContactVerification",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ContactVerification_DeletedById",
                table: "ContactVerification",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_ContactVerification_UpdatedById",
                table: "ContactVerification",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ContactVerification_UserId",
                table: "ContactVerification",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Country_BackendName",
                schema: "lkp",
                table: "Country",
                column: "BackendName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Country_CreatedById",
                schema: "lkp",
                table: "Country",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Country_DeletedById",
                schema: "lkp",
                table: "Country",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_Country_DisplayOrder",
                schema: "lkp",
                table: "Country",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_Country_UpdatedById",
                schema: "lkp",
                table: "Country",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Degree_BackendName",
                schema: "lkp",
                table: "Degree",
                column: "BackendName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Degree_CreatedById",
                schema: "lkp",
                table: "Degree",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Degree_DeletedById",
                schema: "lkp",
                table: "Degree",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_Degree_DisplayOrder",
                schema: "lkp",
                table: "Degree",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_Degree_UpdatedById",
                schema: "lkp",
                table: "Degree",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Department_BackendName",
                schema: "lkp",
                table: "Department",
                column: "BackendName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Department_CreatedById",
                schema: "lkp",
                table: "Department",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Department_DeletedById",
                schema: "lkp",
                table: "Department",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_Department_DisplayOrder",
                schema: "lkp",
                table: "Department",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_Department_ManagementId",
                schema: "lkp",
                table: "Department",
                column: "ManagementId");

            migrationBuilder.CreateIndex(
                name: "IX_Department_UpdatedById",
                schema: "lkp",
                table: "Department",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_EmailQueues_CreatedById",
                table: "EmailQueues",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_EmailQueues_DeletedById",
                table: "EmailQueues",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_EmailQueues_UpdatedById",
                table: "EmailQueues",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_EmailTemplates_CreatedById",
                table: "EmailTemplates",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_EmailTemplates_DeletedById",
                table: "EmailTemplates",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_EmailTemplates_UpdatedById",
                table: "EmailTemplates",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_EntityLog_ChangedByUserId",
                table: "EntityLog",
                column: "ChangedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityLog_CreatedById",
                table: "EntityLog",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_EntityLog_DeletedById",
                table: "EntityLog",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_EntityLog_UpdatedById",
                table: "EntityLog",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Experience_CertificateId",
                schema: "pro",
                table: "Experience",
                column: "CertificateId");

            migrationBuilder.CreateIndex(
                name: "IX_Experience_CountryId",
                schema: "pro",
                table: "Experience",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Experience_CreatedById",
                schema: "pro",
                table: "Experience",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Experience_DeletedById",
                schema: "pro",
                table: "Experience",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_Experience_QualificationId",
                schema: "pro",
                table: "Experience",
                column: "QualificationId");

            migrationBuilder.CreateIndex(
                name: "IX_Experience_UpdatedById",
                schema: "pro",
                table: "Experience",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Experience_UserProfileId",
                schema: "pro",
                table: "Experience",
                column: "UserProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_Gender_BackendName",
                schema: "lkp",
                table: "Gender",
                column: "BackendName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Gender_CreatedById",
                schema: "lkp",
                table: "Gender",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Gender_DeletedById",
                schema: "lkp",
                table: "Gender",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_Gender_DisplayOrder",
                schema: "lkp",
                table: "Gender",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_Gender_UpdatedById",
                schema: "lkp",
                table: "Gender",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_HistoryInvitation_CreatedById",
                table: "HistoryInvitation",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_HistoryInvitation_DeletedById",
                table: "HistoryInvitation",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_HistoryInvitation_InvitationId",
                table: "HistoryInvitation",
                column: "InvitationId");

            migrationBuilder.CreateIndex(
                name: "IX_HistoryInvitation_InvitationStatusId",
                table: "HistoryInvitation",
                column: "InvitationStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_HistoryInvitation_UpdatedById",
                table: "HistoryInvitation",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Invitations_ApplicantId",
                table: "Invitations",
                column: "ApplicantId");

            migrationBuilder.CreateIndex(
                name: "IX_Invitations_CreatedById",
                table: "Invitations",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Invitations_DeletedById",
                table: "Invitations",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_Invitations_InvitationStatusId",
                table: "Invitations",
                column: "InvitationStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Invitations_JobId",
                table: "Invitations",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_Invitations_UpdatedById",
                table: "Invitations",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_InvitationStatus_BackendName",
                schema: "lkp",
                table: "InvitationStatus",
                column: "BackendName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InvitationStatus_CreatedById",
                schema: "lkp",
                table: "InvitationStatus",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_InvitationStatus_DeletedById",
                schema: "lkp",
                table: "InvitationStatus",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_InvitationStatus_DisplayOrder",
                schema: "lkp",
                table: "InvitationStatus",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_InvitationStatus_UpdatedById",
                schema: "lkp",
                table: "InvitationStatus",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Job_ClosingDate",
                schema: "hr",
                table: "Job",
                column: "ClosingDate");

            migrationBuilder.CreateIndex(
                name: "IX_Job_CreatedById",
                schema: "hr",
                table: "Job",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Job_Deleted_ClosingDate",
                schema: "hr",
                table: "Job",
                columns: new[] { "IsDeleted", "ClosingDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Job_Deleted_Status",
                schema: "hr",
                table: "Job",
                columns: new[] { "IsDeleted", "JobStatusId" });

            migrationBuilder.CreateIndex(
                name: "IX_Job_DeletedById",
                schema: "hr",
                table: "Job",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_Job_DepartmentId",
                schema: "hr",
                table: "Job",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Job_GenderId",
                schema: "hr",
                table: "Job",
                column: "GenderId");

            migrationBuilder.CreateIndex(
                name: "IX_Job_JobCategoryId",
                schema: "hr",
                table: "Job",
                column: "JobCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Job_JobStatusId",
                schema: "hr",
                table: "Job",
                column: "JobStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Job_MajorId",
                schema: "hr",
                table: "Job",
                column: "MajorId");

            migrationBuilder.CreateIndex(
                name: "IX_Job_ManagementId",
                schema: "hr",
                table: "Job",
                column: "ManagementId");

            migrationBuilder.CreateIndex(
                name: "IX_Job_SectorId",
                schema: "hr",
                table: "Job",
                column: "SectorId");

            migrationBuilder.CreateIndex(
                name: "IX_Job_Status_ClosingDate_Deleted",
                schema: "hr",
                table: "Job",
                columns: new[] { "JobStatusId", "ClosingDate", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_Job_SubMajorId",
                schema: "hr",
                table: "Job",
                column: "SubMajorId");

            migrationBuilder.CreateIndex(
                name: "IX_Job_Unique_Title_Department_Category_SubMajor",
                schema: "hr",
                table: "Job",
                columns: new[] { "TitleAr", "DepartmentId", "JobCategoryId", "SubMajorId" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Job_UpdatedById",
                schema: "hr",
                table: "Job",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Job_WorkLocationId",
                schema: "hr",
                table: "Job",
                column: "WorkLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Job_WorkTypeId",
                schema: "hr",
                table: "Job",
                column: "WorkTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_JobCategory_BackendName",
                schema: "lkp",
                table: "JobCategory",
                column: "BackendName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JobCategory_CreatedById",
                schema: "lkp",
                table: "JobCategory",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_JobCategory_DeletedById",
                schema: "lkp",
                table: "JobCategory",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_JobCategory_DisplayOrder",
                schema: "lkp",
                table: "JobCategory",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_JobCategory_UpdatedById",
                schema: "lkp",
                table: "JobCategory",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_JobCondition_CreatedById",
                schema: "hr",
                table: "JobCondition",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_JobCondition_DeletedById",
                schema: "hr",
                table: "JobCondition",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_JobCondition_JobId",
                schema: "hr",
                table: "JobCondition",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_JobCondition_UpdatedById",
                schema: "hr",
                table: "JobCondition",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_JobDegree_CreatedById",
                schema: "hr",
                table: "JobDegree",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_JobDegree_DegreeId",
                schema: "hr",
                table: "JobDegree",
                column: "DegreeId");

            migrationBuilder.CreateIndex(
                name: "IX_JobDegree_DeletedById",
                schema: "hr",
                table: "JobDegree",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_JobDegree_JobId",
                schema: "hr",
                table: "JobDegree",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_JobDegree_UpdatedById",
                schema: "hr",
                table: "JobDegree",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_JobRequiredAttachment_CreatedById",
                schema: "hr",
                table: "JobRequiredAttachment",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_JobRequiredAttachment_DeletedById",
                schema: "hr",
                table: "JobRequiredAttachment",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_JobRequiredAttachment_JobId",
                schema: "hr",
                table: "JobRequiredAttachment",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_JobRequiredAttachment_UpdatedById",
                schema: "hr",
                table: "JobRequiredAttachment",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_JobResponsibility_CreatedById",
                schema: "hr",
                table: "JobResponsibility",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_JobResponsibility_DeletedById",
                schema: "hr",
                table: "JobResponsibility",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_JobResponsibility_JobId",
                schema: "hr",
                table: "JobResponsibility",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_JobResponsibility_UpdatedById",
                schema: "hr",
                table: "JobResponsibility",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_JobSkill_CreatedById",
                schema: "hr",
                table: "JobSkill",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_JobSkill_DeletedById",
                schema: "hr",
                table: "JobSkill",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_JobSkill_JobId",
                schema: "hr",
                table: "JobSkill",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_JobSkill_SkillId",
                schema: "hr",
                table: "JobSkill",
                column: "SkillId");

            migrationBuilder.CreateIndex(
                name: "IX_JobSkill_UpdatedById",
                schema: "hr",
                table: "JobSkill",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_JobStatus_BackendName",
                schema: "lkp",
                table: "JobStatus",
                column: "BackendName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JobStatus_CreatedById",
                schema: "lkp",
                table: "JobStatus",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_JobStatus_DeletedById",
                schema: "lkp",
                table: "JobStatus",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_JobStatus_DisplayOrder",
                schema: "lkp",
                table: "JobStatus",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_JobStatus_UpdatedById",
                schema: "lkp",
                table: "JobStatus",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_JobTabReviewAttachment_AttachmentId",
                schema: "hr",
                table: "JobTabReviewAttachment",
                column: "AttachmentId");

            migrationBuilder.CreateIndex(
                name: "IX_JobTabReviewAttachment_CreatedById",
                schema: "hr",
                table: "JobTabReviewAttachment",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_JobTabReviewAttachment_DeletedById",
                schema: "hr",
                table: "JobTabReviewAttachment",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_JobTabReviewAttachment_JobTabReviewNoteId",
                schema: "hr",
                table: "JobTabReviewAttachment",
                column: "JobTabReviewNoteId");

            migrationBuilder.CreateIndex(
                name: "IX_JobTabReviewAttachment_UpdatedById",
                schema: "hr",
                table: "JobTabReviewAttachment",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_JobTabReviewNote_CreatedById",
                schema: "hr",
                table: "JobTabReviewNote",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_JobTabReviewNote_DeletedById",
                schema: "hr",
                table: "JobTabReviewNote",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_JobTabReviewNote_JobId_Tab_ReviewCycleId",
                schema: "hr",
                table: "JobTabReviewNote",
                columns: new[] { "JobId", "Tab", "ReviewCycleId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JobTabReviewNote_UpdatedById",
                schema: "hr",
                table: "JobTabReviewNote",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Language_BackendName",
                schema: "lkp",
                table: "Language",
                column: "BackendName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Language_CreatedById",
                schema: "lkp",
                table: "Language",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Language_DeletedById",
                schema: "lkp",
                table: "Language",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_Language_DisplayOrder",
                schema: "lkp",
                table: "Language",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_Language_UpdatedById",
                schema: "lkp",
                table: "Language",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_LanguageLevel_BackendName",
                schema: "lkp",
                table: "LanguageLevel",
                column: "BackendName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LanguageLevel_CreatedById",
                schema: "lkp",
                table: "LanguageLevel",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_LanguageLevel_DeletedById",
                schema: "lkp",
                table: "LanguageLevel",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_LanguageLevel_DisplayOrder",
                schema: "lkp",
                table: "LanguageLevel",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_LanguageLevel_UpdatedById",
                schema: "lkp",
                table: "LanguageLevel",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_LoginAttempt_CreatedById",
                table: "LoginAttempt",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_LoginAttempt_DeletedById",
                table: "LoginAttempt",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_LoginAttempt_UpdatedById",
                table: "LoginAttempt",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_LoginAttempt_UserId",
                table: "LoginAttempt",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_LoginAttempt_UserTypeId",
                table: "LoginAttempt",
                column: "UserTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Major_BackendName",
                schema: "lkp",
                table: "Major",
                column: "BackendName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Major_CreatedById",
                schema: "lkp",
                table: "Major",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Major_DeletedById",
                schema: "lkp",
                table: "Major",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_Major_DisplayOrder",
                schema: "lkp",
                table: "Major",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_Major_ParentId",
                schema: "lkp",
                table: "Major",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Major_UpdatedById",
                schema: "lkp",
                table: "Major",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Management_BackendName",
                schema: "lkp",
                table: "Management",
                column: "BackendName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Management_CreatedById",
                schema: "lkp",
                table: "Management",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Management_DeletedById",
                schema: "lkp",
                table: "Management",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_Management_DisplayOrder",
                schema: "lkp",
                table: "Management",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_Management_SectorId",
                schema: "lkp",
                table: "Management",
                column: "SectorId");

            migrationBuilder.CreateIndex(
                name: "IX_Management_UpdatedById",
                schema: "lkp",
                table: "Management",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_MaritalStatus_BackendName",
                schema: "lkp",
                table: "MaritalStatus",
                column: "BackendName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MaritalStatus_CreatedById",
                schema: "lkp",
                table: "MaritalStatus",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_MaritalStatus_DeletedById",
                schema: "lkp",
                table: "MaritalStatus",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_MaritalStatus_DisplayOrder",
                schema: "lkp",
                table: "MaritalStatus",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_MaritalStatus_UpdatedById",
                schema: "lkp",
                table: "MaritalStatus",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_CreatedById",
                table: "Notifications",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_DeletedById",
                table: "Notifications",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_Status_Channel",
                table: "Notifications",
                columns: new[] { "Status", "Channel" });

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UpdatedById",
                table: "Notifications",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId",
                table: "Notifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Office_BackendName",
                schema: "lkp",
                table: "Office",
                column: "BackendName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Office_Code",
                schema: "lkp",
                table: "Office",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Office_CountryId",
                schema: "lkp",
                table: "Office",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Office_CreatedById",
                schema: "lkp",
                table: "Office",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Office_DeletedById",
                schema: "lkp",
                table: "Office",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_Office_DisplayOrder",
                schema: "lkp",
                table: "Office",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_Office_UpdatedById",
                schema: "lkp",
                table: "Office",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileAdditionalAttachment_AttachmentId",
                schema: "pro",
                table: "ProfileAdditionalAttachment",
                column: "AttachmentId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileAdditionalAttachment_CreatedById",
                schema: "pro",
                table: "ProfileAdditionalAttachment",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileAdditionalAttachment_DeletedById",
                schema: "pro",
                table: "ProfileAdditionalAttachment",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileAdditionalAttachment_UpdatedById",
                schema: "pro",
                table: "ProfileAdditionalAttachment",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileAdditionalAttachment_UserProfileId",
                schema: "pro",
                table: "ProfileAdditionalAttachment",
                column: "UserProfileId");

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

            migrationBuilder.CreateIndex(
                name: "IX_ProfileChange_CreatedById",
                schema: "hr",
                table: "ProfileChange",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileChange_DeletedById",
                schema: "hr",
                table: "ProfileChange",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileChange_UpdatedById",
                schema: "hr",
                table: "ProfileChange",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileChange_UserProfileId",
                schema: "hr",
                table: "ProfileChange",
                column: "UserProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileLanguage_CreatedById",
                schema: "pro",
                table: "ProfileLanguage",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileLanguage_DeletedById",
                schema: "pro",
                table: "ProfileLanguage",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileLanguage_LanguageId",
                schema: "pro",
                table: "ProfileLanguage",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileLanguage_ReadingLevelId",
                schema: "pro",
                table: "ProfileLanguage",
                column: "ReadingLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileLanguage_SpeakingLevelId",
                schema: "pro",
                table: "ProfileLanguage",
                column: "SpeakingLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileLanguage_UpdatedById",
                schema: "pro",
                table: "ProfileLanguage",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileLanguage_UserProfileId",
                schema: "pro",
                table: "ProfileLanguage",
                column: "UserProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileLanguage_WritingLevelId",
                schema: "pro",
                table: "ProfileLanguage",
                column: "WritingLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileReviewDecision_AttachmentResourceId",
                schema: "hr",
                table: "ProfileReviewDecision",
                column: "AttachmentResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileReviewDecision_CreatedById",
                schema: "hr",
                table: "ProfileReviewDecision",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileReviewDecision_DeletedById",
                schema: "hr",
                table: "ProfileReviewDecision",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileReviewDecision_UpdatedById",
                schema: "hr",
                table: "ProfileReviewDecision",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileReviewDecision_UserProfileId",
                schema: "hr",
                table: "ProfileReviewDecision",
                column: "UserProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileSkill_CreatedById",
                schema: "pro",
                table: "ProfileSkill",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileSkill_DeletedById",
                schema: "pro",
                table: "ProfileSkill",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileSkill_LevelId",
                schema: "pro",
                table: "ProfileSkill",
                column: "LevelId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileSkill_SkillId",
                schema: "pro",
                table: "ProfileSkill",
                column: "SkillId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileSkill_UpdatedById",
                schema: "pro",
                table: "ProfileSkill",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileSkill_UserProfileId",
                schema: "pro",
                table: "ProfileSkill",
                column: "UserProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileSubmission_CreatedById",
                schema: "pro",
                table: "ProfileSubmission",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileSubmission_DeletedById",
                schema: "pro",
                table: "ProfileSubmission",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileSubmission_UpdatedById",
                schema: "pro",
                table: "ProfileSubmission",
                column: "UpdatedById");

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

            migrationBuilder.CreateIndex(
                name: "IX_Qualification_CertificateId",
                schema: "pro",
                table: "Qualification",
                column: "CertificateId");

            migrationBuilder.CreateIndex(
                name: "IX_Qualification_CountryId",
                schema: "pro",
                table: "Qualification",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Qualification_CreatedById",
                schema: "pro",
                table: "Qualification",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Qualification_DegreeId",
                schema: "pro",
                table: "Qualification",
                column: "DegreeId");

            migrationBuilder.CreateIndex(
                name: "IX_Qualification_DeletedById",
                schema: "pro",
                table: "Qualification",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_Qualification_MajorId",
                schema: "pro",
                table: "Qualification",
                column: "MajorId");

            migrationBuilder.CreateIndex(
                name: "IX_Qualification_RatingId",
                schema: "pro",
                table: "Qualification",
                column: "RatingId");

            migrationBuilder.CreateIndex(
                name: "IX_Qualification_StudyTypeId",
                schema: "pro",
                table: "Qualification",
                column: "StudyTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Qualification_SubMajorId",
                schema: "pro",
                table: "Qualification",
                column: "SubMajorId");

            migrationBuilder.CreateIndex(
                name: "IX_Qualification_UniversityId",
                schema: "pro",
                table: "Qualification",
                column: "UniversityId");

            migrationBuilder.CreateIndex(
                name: "IX_Qualification_UpdatedById",
                schema: "pro",
                table: "Qualification",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Qualification_UserProfileId",
                schema: "pro",
                table: "Qualification",
                column: "UserProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_RatingGrade_BackendName",
                schema: "lkp",
                table: "RatingGrade",
                column: "BackendName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RatingGrade_CreatedById",
                schema: "lkp",
                table: "RatingGrade",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_RatingGrade_DeletedById",
                schema: "lkp",
                table: "RatingGrade",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_RatingGrade_DisplayOrder",
                schema: "lkp",
                table: "RatingGrade",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_RatingGrade_UpdatedById",
                schema: "lkp",
                table: "RatingGrade",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshToken_CreatedById",
                table: "RefreshToken",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshToken_DeletedById",
                table: "RefreshToken",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshToken_UpdatedById",
                table: "RefreshToken",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshToken_UserId",
                table: "RefreshToken",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Religion_BackendName",
                schema: "lkp",
                table: "Religion",
                column: "BackendName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Religion_CreatedById",
                schema: "lkp",
                table: "Religion",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Religion_DeletedById",
                schema: "lkp",
                table: "Religion",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_Religion_DisplayOrder",
                schema: "lkp",
                table: "Religion",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_Religion_UpdatedById",
                schema: "lkp",
                table: "Religion",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ResidenceAddress_CertificateId",
                schema: "pro",
                table: "ResidenceAddress",
                column: "CertificateId");

            migrationBuilder.CreateIndex(
                name: "IX_ResidenceAddress_CreatedById",
                schema: "pro",
                table: "ResidenceAddress",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ResidenceAddress_DeletedById",
                schema: "pro",
                table: "ResidenceAddress",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_ResidenceAddress_UpdatedById",
                schema: "pro",
                table: "ResidenceAddress",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Resources_CreatedById",
                table: "Resources",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Resources_DeletedById",
                table: "Resources",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_Resources_Key",
                table: "Resources",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Resources_UpdatedById",
                table: "Resources",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewItem_CreatedById",
                schema: "hr",
                table: "ReviewItem",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewItem_DeletedById",
                schema: "hr",
                table: "ReviewItem",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewItem_ProfileChangeId",
                schema: "hr",
                table: "ReviewItem",
                column: "ProfileChangeId");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewItem_UpdatedById",
                schema: "hr",
                table: "ReviewItem",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewItem_UserProfileId",
                schema: "hr",
                table: "ReviewItem",
                column: "UserProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_Sector_BackendName",
                schema: "lkp",
                table: "Sector",
                column: "BackendName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sector_CreatedById",
                schema: "lkp",
                table: "Sector",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Sector_DeletedById",
                schema: "lkp",
                table: "Sector",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_Sector_DisplayOrder",
                schema: "lkp",
                table: "Sector",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_Sector_UpdatedById",
                schema: "lkp",
                table: "Sector",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Skill_CreatedById",
                schema: "lkp",
                table: "Skill",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Skill_DeletedById",
                schema: "lkp",
                table: "Skill",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_Skill_MajorId",
                schema: "lkp",
                table: "Skill",
                column: "MajorId");

            migrationBuilder.CreateIndex(
                name: "IX_Skill_SkillRequirementTypeId",
                schema: "lkp",
                table: "Skill",
                column: "SkillRequirementTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Skill_SkillTypeId",
                schema: "lkp",
                table: "Skill",
                column: "SkillTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Skill_UpdatedById",
                schema: "lkp",
                table: "Skill",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_SkillLevel_BackendName",
                schema: "lkp",
                table: "SkillLevel",
                column: "BackendName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SkillLevel_CreatedById",
                schema: "lkp",
                table: "SkillLevel",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_SkillLevel_DeletedById",
                schema: "lkp",
                table: "SkillLevel",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_SkillLevel_DisplayOrder",
                schema: "lkp",
                table: "SkillLevel",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_SkillLevel_UpdatedById",
                schema: "lkp",
                table: "SkillLevel",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_SkillRequirementType_BackendName",
                schema: "lkp",
                table: "SkillRequirementType",
                column: "BackendName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SkillRequirementType_CreatedById",
                schema: "lkp",
                table: "SkillRequirementType",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_SkillRequirementType_DeletedById",
                schema: "lkp",
                table: "SkillRequirementType",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_SkillRequirementType_DisplayOrder",
                schema: "lkp",
                table: "SkillRequirementType",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_SkillRequirementType_UpdatedById",
                schema: "lkp",
                table: "SkillRequirementType",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_SkillType_BackendName",
                schema: "lkp",
                table: "SkillType",
                column: "BackendName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SkillType_CreatedById",
                schema: "lkp",
                table: "SkillType",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_SkillType_DeletedById",
                schema: "lkp",
                table: "SkillType",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_SkillType_DisplayOrder",
                schema: "lkp",
                table: "SkillType",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_SkillType_UpdatedById",
                schema: "lkp",
                table: "SkillType",
                column: "UpdatedById");

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

            migrationBuilder.CreateIndex(
                name: "IX_StudyType_BackendName",
                schema: "lkp",
                table: "StudyType",
                column: "BackendName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudyType_CreatedById",
                schema: "lkp",
                table: "StudyType",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_StudyType_DeletedById",
                schema: "lkp",
                table: "StudyType",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_StudyType_DisplayOrder",
                schema: "lkp",
                table: "StudyType",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_StudyType_UpdatedById",
                schema: "lkp",
                table: "StudyType",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_TargetEntity_BackendName",
                schema: "lkp",
                table: "TargetEntity",
                column: "BackendName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TargetEntity_CreatedById",
                schema: "lkp",
                table: "TargetEntity",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_TargetEntity_DeletedById",
                schema: "lkp",
                table: "TargetEntity",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_TargetEntity_DisplayOrder",
                schema: "lkp",
                table: "TargetEntity",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_TargetEntity_UpdatedById",
                schema: "lkp",
                table: "TargetEntity",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingCourse_CertificateId",
                schema: "pro",
                table: "TrainingCourse",
                column: "CertificateId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingCourse_CountryId",
                schema: "pro",
                table: "TrainingCourse",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingCourse_CreatedById",
                schema: "pro",
                table: "TrainingCourse",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingCourse_DeletedById",
                schema: "pro",
                table: "TrainingCourse",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingCourse_UpdatedById",
                schema: "pro",
                table: "TrainingCourse",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingCourse_UserProfileId",
                schema: "pro",
                table: "TrainingCourse",
                column: "UserProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_University_BackendName",
                schema: "lkp",
                table: "University",
                column: "BackendName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_University_CityId",
                schema: "lkp",
                table: "University",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_University_CreatedById",
                schema: "lkp",
                table: "University",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_University_DeletedById",
                schema: "lkp",
                table: "University",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_University_DisplayOrder",
                schema: "lkp",
                table: "University",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_University_UpdatedById",
                schema: "lkp",
                table: "University",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfile_BirthdayCertificateId",
                schema: "app",
                table: "UserProfile",
                column: "BirthdayCertificateId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfile_CandidateTypeId",
                schema: "app",
                table: "UserProfile",
                column: "CandidateTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfile_CreatedById",
                schema: "app",
                table: "UserProfile",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfile_DeletedById",
                schema: "app",
                table: "UserProfile",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfile_GenderId",
                schema: "app",
                table: "UserProfile",
                column: "GenderId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfile_InterviewLocationId",
                schema: "app",
                table: "UserProfile",
                column: "InterviewLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfile_MaritalStatusId",
                schema: "app",
                table: "UserProfile",
                column: "MaritalStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfile_MarriageCertificateId",
                schema: "app",
                table: "UserProfile",
                column: "MarriageCertificateId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfile_NationalCardId",
                schema: "app",
                table: "UserProfile",
                column: "NationalCardId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfile_NationalityId",
                schema: "app",
                table: "UserProfile",
                column: "NationalityId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfile_NationalNumber_NationalityId",
                schema: "app",
                table: "UserProfile",
                columns: new[] { "NationalNumber", "NationalityId" },
                unique: true,
                filter: "[NationalNumber] IS NOT NULL AND [NationalityId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfile_OfficeId",
                schema: "app",
                table: "UserProfile",
                column: "OfficeId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfile_ReligionId",
                schema: "app",
                table: "UserProfile",
                column: "ReligionId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfile_ResidenceAddressId",
                schema: "app",
                table: "UserProfile",
                column: "ResidenceAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfile_ResidenceCountryId",
                schema: "app",
                table: "UserProfile",
                column: "ResidenceCountryId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfile_ResumeAttachmentId",
                schema: "app",
                table: "UserProfile",
                column: "ResumeAttachmentId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfile_SponsorProfileId",
                schema: "app",
                table: "UserProfile",
                column: "SponsorProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfile_TargetEntityId",
                schema: "app",
                table: "UserProfile",
                column: "TargetEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfile_UpdatedById",
                schema: "app",
                table: "UserProfile",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfile_UserId",
                schema: "app",
                table: "UserProfile",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserSession_CreatedById",
                table: "UserSession",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_UserSession_DeletedById",
                table: "UserSession",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_UserSession_UpdatedById",
                table: "UserSession",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_UserSession_UserId",
                table: "UserSession",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserType_BackendName",
                schema: "lkp",
                table: "UserType",
                column: "BackendName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserType_CreatedById",
                schema: "lkp",
                table: "UserType",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_UserType_DeletedById",
                schema: "lkp",
                table: "UserType",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_UserType_DisplayOrder",
                schema: "lkp",
                table: "UserType",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_UserType_UpdatedById",
                schema: "lkp",
                table: "UserType",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_WorkType_BackendName",
                schema: "lkp",
                table: "WorkType",
                column: "BackendName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkType_CreatedById",
                schema: "lkp",
                table: "WorkType",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_WorkType_DeletedById",
                schema: "lkp",
                table: "WorkType",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_WorkType_DisplayOrder",
                schema: "lkp",
                table: "WorkType",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_WorkType_UpdatedById",
                schema: "lkp",
                table: "WorkType",
                column: "UpdatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Achievement_AchievementType_AchievementTypeId",
                schema: "pro",
                table: "Achievement",
                column: "AchievementTypeId",
                principalSchema: "lkp",
                principalTable: "AchievementType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Achievement_AspNetUsers_CreatedById",
                schema: "pro",
                table: "Achievement",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Achievement_AspNetUsers_DeletedById",
                schema: "pro",
                table: "Achievement",
                column: "DeletedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Achievement_AspNetUsers_UpdatedById",
                schema: "pro",
                table: "Achievement",
                column: "UpdatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Achievement_Country_CountryId",
                schema: "pro",
                table: "Achievement",
                column: "CountryId",
                principalSchema: "lkp",
                principalTable: "Country",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Achievement_Resources_AttachmentId",
                schema: "pro",
                table: "Achievement",
                column: "AttachmentId",
                principalTable: "Resources",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Achievement_UserProfile_UserProfileId",
                schema: "pro",
                table: "Achievement",
                column: "UserProfileId",
                principalSchema: "app",
                principalTable: "UserProfile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AchievementType_AspNetUsers_CreatedById",
                schema: "lkp",
                table: "AchievementType",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AchievementType_AspNetUsers_DeletedById",
                schema: "lkp",
                table: "AchievementType",
                column: "DeletedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AchievementType_AspNetUsers_UpdatedById",
                schema: "lkp",
                table: "AchievementType",
                column: "UpdatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                table: "AspNetUserClaims",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                table: "AspNetUserLogins",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                table: "AspNetUserRoles",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_UserType_UserTypeId",
                table: "AspNetUsers",
                column: "UserTypeId",
                principalSchema: "lkp",
                principalTable: "UserType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserType_AspNetUsers_CreatedById",
                schema: "lkp",
                table: "UserType");

            migrationBuilder.DropForeignKey(
                name: "FK_UserType_AspNetUsers_DeletedById",
                schema: "lkp",
                table: "UserType");

            migrationBuilder.DropForeignKey(
                name: "FK_UserType_AspNetUsers_UpdatedById",
                schema: "lkp",
                table: "UserType");

            migrationBuilder.DropTable(
                name: "Achievement",
                schema: "pro");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "AuditTrailEntry",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "CandidateTypeProviderLogin",
                schema: "lkp");

            migrationBuilder.DropTable(
                name: "ContactVerification");

            migrationBuilder.DropTable(
                name: "EmailQueues");

            migrationBuilder.DropTable(
                name: "EmailTemplates");

            migrationBuilder.DropTable(
                name: "EntityLog");

            migrationBuilder.DropTable(
                name: "Experience",
                schema: "pro");

            migrationBuilder.DropTable(
                name: "HistoryInvitation");

            migrationBuilder.DropTable(
                name: "JobCondition",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "JobDegree",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "JobRequiredAttachment",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "JobResponsibility",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "JobSkill",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "JobTabReviewAttachment",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "LoginAttempt");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "ProfileAdditionalAttachment",
                schema: "pro");

            migrationBuilder.DropTable(
                name: "ProfileAssignment",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "ProfileLanguage",
                schema: "pro");

            migrationBuilder.DropTable(
                name: "ProfileReviewDecision",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "ProfileSkill",
                schema: "pro");

            migrationBuilder.DropTable(
                name: "ProfileSubmission",
                schema: "pro");

            migrationBuilder.DropTable(
                name: "RefreshToken");

            migrationBuilder.DropTable(
                name: "ReviewItem",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "TrainingCourse",
                schema: "pro");

            migrationBuilder.DropTable(
                name: "UserSession");

            migrationBuilder.DropTable(
                name: "AchievementType",
                schema: "lkp");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "ProviderLogin",
                schema: "lkp");

            migrationBuilder.DropTable(
                name: "Qualification",
                schema: "pro");

            migrationBuilder.DropTable(
                name: "Invitations");

            migrationBuilder.DropTable(
                name: "Skill",
                schema: "lkp");

            migrationBuilder.DropTable(
                name: "JobTabReviewNote",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "LanguageLevel",
                schema: "lkp");

            migrationBuilder.DropTable(
                name: "Language",
                schema: "lkp");

            migrationBuilder.DropTable(
                name: "SkillLevel",
                schema: "lkp");

            migrationBuilder.DropTable(
                name: "ProfileChange",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "Degree",
                schema: "lkp");

            migrationBuilder.DropTable(
                name: "RatingGrade",
                schema: "lkp");

            migrationBuilder.DropTable(
                name: "StudyType",
                schema: "lkp");

            migrationBuilder.DropTable(
                name: "University",
                schema: "lkp");

            migrationBuilder.DropTable(
                name: "InvitationStatus",
                schema: "lkp");

            migrationBuilder.DropTable(
                name: "SkillRequirementType",
                schema: "lkp");

            migrationBuilder.DropTable(
                name: "SkillType",
                schema: "lkp");

            migrationBuilder.DropTable(
                name: "Job",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "UserProfile",
                schema: "app");

            migrationBuilder.DropTable(
                name: "City",
                schema: "lkp");

            migrationBuilder.DropTable(
                name: "Department",
                schema: "lkp");

            migrationBuilder.DropTable(
                name: "JobCategory",
                schema: "lkp");

            migrationBuilder.DropTable(
                name: "JobStatus",
                schema: "lkp");

            migrationBuilder.DropTable(
                name: "Major",
                schema: "lkp");

            migrationBuilder.DropTable(
                name: "WorkType",
                schema: "lkp");

            migrationBuilder.DropTable(
                name: "CandidateType",
                schema: "lkp");

            migrationBuilder.DropTable(
                name: "Gender",
                schema: "lkp");

            migrationBuilder.DropTable(
                name: "MaritalStatus",
                schema: "lkp");

            migrationBuilder.DropTable(
                name: "Office",
                schema: "lkp");

            migrationBuilder.DropTable(
                name: "Religion",
                schema: "lkp");

            migrationBuilder.DropTable(
                name: "ResidenceAddress",
                schema: "pro");

            migrationBuilder.DropTable(
                name: "SponsorProfile",
                schema: "app");

            migrationBuilder.DropTable(
                name: "TargetEntity",
                schema: "lkp");

            migrationBuilder.DropTable(
                name: "Management",
                schema: "lkp");

            migrationBuilder.DropTable(
                name: "Country",
                schema: "lkp");

            migrationBuilder.DropTable(
                name: "Resources");

            migrationBuilder.DropTable(
                name: "SponsorType",
                schema: "lkp");

            migrationBuilder.DropTable(
                name: "Sector",
                schema: "lkp");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "UserType",
                schema: "lkp");
        }
    }
}
