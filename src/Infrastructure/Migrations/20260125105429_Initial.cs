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
                    IsSystemRole = table.Column<bool>(type: "bit", nullable: false),
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
                    Title = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    IssuingAuthority = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    CountryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IssueDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
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
                    NameAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DescriptionAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DescriptionEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    BackendName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
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
                    AgreedToTerms = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    LastLoginDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
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
                    OtpExpiry = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    OtpAttempts = table.Column<int>(type: "int", nullable: false),
                    OtpLockedUntilUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    OtpSendWindowStartUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    OtpSendsInWindow = table.Column<int>(type: "int", nullable: false),
                    Discriminator = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: false),
                    EmployeeProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    OfficeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
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
                    NameAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DescriptionAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DescriptionEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    BackendName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
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
                    NameAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DescriptionAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DescriptionEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    BackendName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
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
                    NameAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DescriptionAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DescriptionEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    BackendName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
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
                    RecipientEmail = table.Column<string>(type: "nvarchar(254)", maxLength: 254, nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Body = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    IsSent = table.Column<bool>(type: "bit", nullable: false),
                    SentDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    RetryCount = table.Column<int>(type: "int", nullable: false),
                    ErrorMessage = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true)
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
                    TemplateKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    BodyTemplate = table.Column<string>(type: "nvarchar(max)", maxLength: 8000, nullable: false),
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
                name: "EmployeeProfile",
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
                    EmployeeNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Qid = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    FullNameAr = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    FullNameEn = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(254)", maxLength: 254, nullable: true),
                    MobileNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Department = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DepartmentNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Section = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    SectionNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    JobTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    RawPayload = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeProfile", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeProfile_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeProfile_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeProfile_AspNetUsers_UpdatedById",
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
                    NameAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DescriptionAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DescriptionEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    BackendName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
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
                    NameAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DescriptionAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DescriptionEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    BackendName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
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
                    NameAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DescriptionAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DescriptionEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    BackendName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
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
                name: "JobCategoryCandidateSettings",
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
                    AcademicJobVacancies = table.Column<int>(type: "int", nullable: false),
                    LaborJobVacancies = table.Column<int>(type: "int", nullable: false),
                    AdministrativeJobVacancies = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobCategoryCandidateSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobCategoryCandidateSettings_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobCategoryCandidateSettings_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobCategoryCandidateSettings_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "JobPointConfiguration",
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
                    ApplicantCategoryMaxPoints = table.Column<int>(type: "int", nullable: false),
                    EducationMaxPoints = table.Column<int>(type: "int", nullable: false),
                    ExperienceMaxPoints = table.Column<int>(type: "int", nullable: false),
                    TrainingMaxPoints = table.Column<int>(type: "int", nullable: false),
                    CertificatesMaxPoints = table.Column<int>(type: "int", nullable: false),
                    SkillsMaxPoints = table.Column<int>(type: "int", nullable: false),
                    LanguagesMaxPoints = table.Column<int>(type: "int", nullable: false),
                    MaxPoints = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobPointConfiguration", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobPointConfiguration_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobPointConfiguration_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobPointConfiguration_AspNetUsers_UpdatedById",
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
                    NameAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DescriptionAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DescriptionEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    BackendName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
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
                name: "KawaderQids",
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
                    Qid = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KawaderQids", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KawaderQids_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_KawaderQids_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_KawaderQids_AspNetUsers_UpdatedById",
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
                    NameAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DescriptionAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DescriptionEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    BackendName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
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
                    NameAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DescriptionAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DescriptionEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    BackendName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
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
                    NameAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DescriptionAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DescriptionEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    BackendName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
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
                    NameAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DescriptionAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DescriptionEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    BackendName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
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
                    CcAddress = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Channel = table.Column<int>(type: "int", nullable: false),
                    TemplateKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Body = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    PayloadJson = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ProviderMessageId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
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
                name: "Permission",
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
                    NameAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DescriptionAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DescriptionEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    BackendName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permission", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Permission_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Permission_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Permission_AspNetUsers_UpdatedById",
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
                    NameAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DescriptionAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DescriptionEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    BackendName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
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
                    NameAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DescriptionAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DescriptionEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    BackendName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
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
                    Token = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    Expires = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedByIp = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: true),
                    RevokedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    RevokedByIp = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: true),
                    ReplacedByToken = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    RevokedReason = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UserDeviceId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SecurityStamp = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false)
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
                    NameAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DescriptionAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DescriptionEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    BackendName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
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
                    NameAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DescriptionAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DescriptionEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    BackendName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
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
                    NameAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DescriptionAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DescriptionEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    BackendName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
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
                    NameAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DescriptionAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DescriptionEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    BackendName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
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
                    NameAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DescriptionAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DescriptionEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    BackendName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
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
                    NameAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DescriptionAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DescriptionEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    BackendName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
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
                    NameAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DescriptionAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DescriptionEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    BackendName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
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
                    NameAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DescriptionAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DescriptionEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    BackendName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
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
                    NameAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DescriptionAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DescriptionEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    BackendName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
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
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DescriptionAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DescriptionEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    BackendName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
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
                    OfficeAdminId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PhoneCountryCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    NameAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DescriptionAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DescriptionEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    BackendName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
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
                        name: "FK_Office_AspNetUsers_OfficeAdminId",
                        column: x => x.OfficeAdminId,
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
                    NameAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DescriptionAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DescriptionEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    BackendName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
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
                    SkillTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DescriptionAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DescriptionEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    BackendName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
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
                    Source = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Succeeded = table.Column<bool>(type: "bit", nullable: false),
                    FailureReason = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    SessionId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    AttemptedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: true)
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
                    WebSite = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(254)", maxLength: 254, nullable: true),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    LogoEnId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LogoArId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    OriginalName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NameAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DescriptionAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DescriptionEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    BackendName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
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
                    table.ForeignKey(
                        name: "FK_University_Resources_LogoArId",
                        column: x => x.LogoArId,
                        principalTable: "Resources",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_University_Resources_LogoEnId",
                        column: x => x.LogoEnId,
                        principalTable: "Resources",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "OfficeSupportedCountry",
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
                    OfficeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CountryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OfficeSupportedCountry", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OfficeSupportedCountry_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OfficeSupportedCountry_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OfficeSupportedCountry_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OfficeSupportedCountry_Country_CountryId",
                        column: x => x.CountryId,
                        principalSchema: "lkp",
                        principalTable: "Country",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OfficeSupportedCountry_Office_OfficeId",
                        column: x => x.OfficeId,
                        principalSchema: "lkp",
                        principalTable: "Office",
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
                    NameAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DescriptionAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DescriptionEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    BackendName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
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
                name: "MajorSkill",
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
                    MajorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SkillId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsSkillRequired = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MajorSkill", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MajorSkill_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MajorSkill_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MajorSkill_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MajorSkill_Major_MajorId",
                        column: x => x.MajorId,
                        principalSchema: "lkp",
                        principalTable: "Major",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MajorSkill_Skill_SkillId",
                        column: x => x.SkillId,
                        principalSchema: "lkp",
                        principalTable: "Skill",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                    Provider = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CandidateTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TargetEntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OfficeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ResumeAttachmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    NationalCardId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    NationalNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    QIDExpiry = table.Column<DateOnly>(type: "date", nullable: true),
                    BirthDate = table.Column<DateOnly>(type: "date", nullable: true),
                    NationalityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    GenderId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReligionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    MaritalStatusId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ChildrenCount = table.Column<int>(type: "int", nullable: false),
                    ResidenceCountryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    InterviewLocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ResidenceAddressId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    HasDisability = table.Column<bool>(type: "bit", nullable: false),
                    DisabilityDetails = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    SponsorProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    BirthdayCertificateId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    MarriageCertificateId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "InCreation"),
                    AvailableForRecruitment = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
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
                    OverViewAr = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    OverViewEn = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    BenefitsAr = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    BenefitsEn = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    QualificationDescriptionAr = table.Column<string>(type: "nvarchar(3000)", maxLength: 3000, nullable: true),
                    QualificationDescriptionEn = table.Column<string>(type: "nvarchar(3000)", maxLength: 3000, nullable: true)
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
                    FileName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
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
                name: "ProfileChangeRequest",
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
                    Action = table.Column<int>(type: "int", nullable: false),
                    TargetKey = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    FieldPath = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EntityName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    OldValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NewValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OldResourceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    NewResourceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AttachmentTitle = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    RequestedById = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ReviewedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReviewedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ReviewerNote = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CanceledById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CanceledAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfileChangeRequest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProfileChangeRequest_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProfileChangeRequest_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProfileChangeRequest_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProfileChangeRequest_UserProfile_UserProfileId",
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
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Summary = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
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
                        name: "FK_ProfileSkill_Skill_SkillId",
                        column: x => x.SkillId,
                        principalSchema: "lkp",
                        principalTable: "Skill",
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
                    Title = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Provider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    CountryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
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
                name: "UserProfileLogger",
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
                    PerformedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ActionType = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    Section = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    EntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AttachmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReviewStatus = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProfileLogger", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserProfileLogger_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserProfileLogger_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserProfileLogger_AspNetUsers_PerformedById",
                        column: x => x.PerformedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserProfileLogger_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserProfileLogger_UserProfile_UserProfileId",
                        column: x => x.UserProfileId,
                        principalSchema: "app",
                        principalTable: "UserProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Invitation",
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
                    ApplicantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsAccepted = table.Column<bool>(type: "bit", nullable: false),
                    AcceptedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    InvitationStatusId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BatchNumber = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invitation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Invitation_AspNetUsers_ApplicantId",
                        column: x => x.ApplicantId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Invitation_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Invitation_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Invitation_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Invitation_InvitationStatus_InvitationStatusId",
                        column: x => x.InvitationStatusId,
                        principalSchema: "lkp",
                        principalTable: "InvitationStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Invitation_Job_JobId",
                        column: x => x.JobId,
                        principalSchema: "hr",
                        principalTable: "Job",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "JobCandidateFilterSetting",
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
                    GenderId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    MinimumPoints = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobCandidateFilterSetting", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobCandidateFilterSetting_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobCandidateFilterSetting_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobCandidateFilterSetting_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobCandidateFilterSetting_Gender_GenderId",
                        column: x => x.GenderId,
                        principalSchema: "lkp",
                        principalTable: "Gender",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_JobCandidateFilterSetting_Job_JobId",
                        column: x => x.JobId,
                        principalSchema: "hr",
                        principalTable: "Job",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                    TextAr = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    TextEn = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false)
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
                name: "JobPointsMain",
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
                    ApplicantCategory = table.Column<int>(type: "int", nullable: false),
                    Education = table.Column<int>(type: "int", nullable: false),
                    Experience = table.Column<int>(type: "int", nullable: false),
                    Training = table.Column<int>(type: "int", nullable: false),
                    Certificates = table.Column<int>(type: "int", nullable: false),
                    Skills = table.Column<int>(type: "int", nullable: false),
                    Languages = table.Column<int>(type: "int", nullable: false),
                    Total = table.Column<int>(type: "int", nullable: false),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobPointsMain", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobPointsMain_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobPointsMain_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobPointsMain_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobPointsMain_Job_JobId",
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
                    TextAr = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    TextEn = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
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
                name: "JobReviewAttachment",
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
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ReviewCycleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttachmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobReviewAttachment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobReviewAttachment_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobReviewAttachment_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobReviewAttachment_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobReviewAttachment_Job_JobId",
                        column: x => x.JobId,
                        principalSchema: "hr",
                        principalTable: "Job",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JobReviewAttachment_Resources_AttachmentId",
                        column: x => x.AttachmentId,
                        principalTable: "Resources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                    TargetType = table.Column<int>(type: "int", nullable: false),
                    Section = table.Column<int>(type: "int", nullable: false),
                    FieldPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EntityName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ResourceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AttachmentTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ReviewedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReviewedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ReviewerNote = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApprovedHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CurrentHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsOutdated = table.Column<bool>(type: "bit", nullable: false)
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
                        name: "FK_ReviewItem_ProfileChangeRequest_ProfileChangeId",
                        column: x => x.ProfileChangeId,
                        principalSchema: "hr",
                        principalTable: "ProfileChangeRequest",
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
                    EmployerName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    JobTitle = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CountryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
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
                    Note = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true)
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
                        name: "FK_HistoryInvitation_Invitation_InvitationId",
                        column: x => x.InvitationId,
                        principalSchema: "hr",
                        principalTable: "Invitation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JobCandidateNationalityPercentage",
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
                    JobCandidateFilterSettingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CandidateTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NationalityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Percentage = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobCandidateNationalityPercentage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobCandidateNationalityPercentage_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobCandidateNationalityPercentage_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobCandidateNationalityPercentage_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobCandidateNationalityPercentage_CandidateType_CandidateTypeId",
                        column: x => x.CandidateTypeId,
                        principalSchema: "lkp",
                        principalTable: "CandidateType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JobCandidateNationalityPercentage_Country_NationalityId",
                        column: x => x.NationalityId,
                        principalSchema: "lkp",
                        principalTable: "Country",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JobCandidateNationalityPercentage_JobCandidateFilterSetting_JobCandidateFilterSettingId",
                        column: x => x.JobCandidateFilterSettingId,
                        principalSchema: "hr",
                        principalTable: "JobCandidateFilterSetting",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JobCandidateTypePercentage",
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
                    JobCandidateFilterSettingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CandidateTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Percentage = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobCandidateTypePercentage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobCandidateTypePercentage_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobCandidateTypePercentage_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobCandidateTypePercentage_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobCandidateTypePercentage_CandidateType_CandidateTypeId",
                        column: x => x.CandidateTypeId,
                        principalSchema: "lkp",
                        principalTable: "CandidateType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JobCandidateTypePercentage_JobCandidateFilterSetting_JobCandidateFilterSettingId",
                        column: x => x.JobCandidateFilterSettingId,
                        principalSchema: "hr",
                        principalTable: "JobCandidateFilterSetting",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JobPointsDetail",
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
                    JobPointsMainId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Points = table.Column<int>(type: "int", nullable: false),
                    ReferenceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobPointsDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobPointsDetail_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobPointsDetail_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobPointsDetail_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobPointsDetail_JobPointsMain_JobPointsMainId",
                        column: x => x.JobPointsMainId,
                        principalSchema: "hr",
                        principalTable: "JobPointsMain",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "AchievementType",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsActive", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("3f5f4d1f-b214-44cb-9b9e-2f9ec3c1f7f1"), "Certificate", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "شهادة", "Certificate", 1, true, false, "شهادة", "Certificate", null, null },
                    { new Guid("6c3b1b1b-0c9c-4e0e-8d1e-4d6c12e91533"), "Award", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "جائزة", "Award", 2, true, false, "جائزة", "Award", null, null }
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "DescriptionAr", "DescriptionEn", "IsSystemRole", "Name", "NameAr", "NameEn", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("12f5805d-6970-4a9e-a275-2b7cf3db3bb8"), "d0f6fe80-b684-40c0-9351-727e2feef829", "موظف المكتب العادي", "Office user", true, "OfficeUser", "موظف المكتب", "Office User", "OFFICEUSER" },
                    { new Guid("1361d691-53c5-4a84-aea1-64ff134cf082"), "00ef28cd-ca1e-4a83-9a29-7e6e07c50e78", "مدير النظام الكامل", "Full system administrator", true, "SystemAdmin", "مدير النظام", "System Admin", "SYSTEMADMIN" },
                    { new Guid("5b757ede-93e1-4c7a-88d3-5e89007d648b"), "af759613-b36e-4acc-ab65-3e35abeb3e69", "مدير الموارد البشرية", "HR Manager", true, "HrManager", "مدير الموارد البشرية", "HR Manager", "HRMANAGER" },
                    { new Guid("5f12e420-f666-4af4-a8fa-4e4aa755fdcd"), "82bb983f-1a91-4572-8d70-0598885b0514", "موظف اساسي", "Basic Employee", true, "Employee", "موظف", "Employee", "EMPLOYEE" },
                    { new Guid("98e20970-b6bc-4da9-a947-f75e9adae3ca"), "51961925-7d86-4e6a-bf60-646475d80319", "مدير المكتب والصلاحيات المرتبطة", "Office administrator", true, "OfficeAdmin", "مدير المكتب", "Office Admin", "OFFICEADMIN" },
                    { new Guid("ac011a30-6b0e-496c-a8ef-ba8132bd1808"), "acfb9736-65f0-4112-8645-f989ba91eba1", "مدير القسم", "Department Manager", true, "DepartmentManager", "مدير القسم", "Department Manager", "DEPARTMENTMANAGER" },
                    { new Guid("d8689e0c-d872-42e9-87b7-c3bb27305e07"), "b8d1862f-102c-4e76-9d53-3e9452930ce0", "موظف بصلاحية كاملة", "Employee Super Admin", false, "EmployeeSuperAdmin", "موظف بصلاحية كاملة", "Employee Super Admin", "EMPLOYEESUPERADMIN" }
                });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "CandidateType",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsActive", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("268d49f6-0dda-4dd6-8346-be355c553496"), "Qatari", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "قطري الجنسية", "Qatari National", 1, true, false, "قطري", "Qatari", null, null },
                    { new Guid("42b374d1-8032-44d7-95bd-ff5a64abbc95"), "NonQatari", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "مقيم خارج دولة قطر", "Resident outside Qatar", 4, true, false, "مقيم خارج قطر", "Resident outside Qatar", null, null },
                    { new Guid("50f14c55-d5f0-4bba-930e-ab73b012e6cb"), "GCC", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "مواطن دول مجلس التعاون الخليجي", "Citizen of a GCC country", 2, true, false, "مواطن دول مجلس التعاون الخليجي", "Citizen of a GCC country", null, null },
                    { new Guid("744256ea-4ee0-4a63-b071-8810895a33dc"), "SonOfQatariMother", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "أبناء المرأة القطرية المتزوجة من غير قطري", "Children of a Qatari woman married to a non-Qatari", 5, true, false, "أبناء المرأة القطرية المتزوجة من غير قطري", "Children of a Qatari woman married to a non-Qatari", null, null },
                    { new Guid("7b06bc91-88b3-46ec-b6cc-ef2338864d41"), "ResidentQatar", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "مقيم داخل دولة قطر", "Resident in Qatar", 3, true, false, "مقيم في قطر", "Resident in Qatar", null, null },
                    { new Guid("ce67465e-6fed-4a83-9161-8eba16a79af3"), "WifeOfQatari", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "الزوج أو الزوجة غير قطري/ة المتزوج/ة من قطري/ة", "Non-Qatari spouse married to a Qatari", 6, true, false, "الزوج أو الزوجة غير قطري/ة المتزوج/ة من قطري/ة", "Non-Qatari spouse married to a Qatari", null, null }
                });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "Degree",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsActive", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("1af8c855-975f-4a49-bcf0-343a6d7fd8df"), "PostgraduateDiploma", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "دبلوم دراسات عليا", "Postgraduate Diploma", 0, true, false, "دبلوم دراسات عليا", "Postgraduate Diploma", null, null },
                    { new Guid("6e453f48-5f2f-4f98-8b76-f416cdd4811b"), "Primary", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "ابتدائي", "Primary", 0, true, false, "ابتدائي", "Primary", null, null },
                    { new Guid("8ca14478-019d-4d3e-89cd-90cb89baf463"), "Master", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "ماجستير", "Master", 0, true, false, "ماجستير", "Master's", null, null },
                    { new Guid("d60cb9b1-f0ce-4c0a-a147-51e8d3947ef4"), "Doctorate", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "دكتوراه", "Doctorate", 0, true, false, "دكتوراه", "PhD", null, null },
                    { new Guid("de5901db-60dd-49f0-953c-daf923cf9f4a"), "Bachelor", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "بكالوريوس", "Bachelor", 0, true, false, "بكالوريوس", "Bachelor's", null, null },
                    { new Guid("ebf2faa1-5ce6-4a04-9472-2746bfbbd252"), "Secondary", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "ثانوي", "Secondary", 0, true, false, "ثانوي", "Secondary", null, null },
                    { new Guid("f1a31fe6-ba80-46cb-b24b-f402bcb4fdec"), "Preparatory", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "إعدادي", "Preparatory", 0, true, false, "إعدادي", "Preparatory", null, null },
                    { new Guid("f6249ce2-fa02-4e18-9c89-151ba3be0c12"), "IntermediateDiploma", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "دبلوم متوسط", "Intermediate Diploma", 0, true, false, "دبلوم متوسط", "Intermediate Diploma", null, null }
                });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "Gender",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsActive", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("03cbe4e3-dc47-4d0c-8a07-87917af1d2dd"), "Female", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "أنثى", "Female", 0, true, false, "أنثى", "Female", null, null },
                    { new Guid("4436a8ce-3f1e-4581-be3d-839c67127a9f"), "All", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "جميع الخيارات", "All Options", 0, true, false, "جميعها", "All", null, null },
                    { new Guid("6720e352-b360-41f0-8d3b-fa5116b7a0b4"), "Male", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "ذكر", "Male", 0, true, false, "ذكر", "Male", null, null }
                });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "InvitationStatus",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsActive", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("22ef7e86-28cb-4a30-98bc-7d45f9b44de3"), "Submitted", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "المرشح قدم طلبه وجميع بياناته مكتملة.", "The candidate submitted the application with all required information completed.", 3, true, false, "تم التقديم", "Submitted", null, null },
                    { new Guid("64236c6a-167a-4213-b1d6-80c2c8c86dde"), "Read", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "المرشح فتح الدعوة لأول مرة ولم يبدأ التقديم.", "The candidate opened the invitation for the first time but has not started the application.", 2, true, false, "تمت القراءة", "Read", null, null },
                    { new Guid("7103ba49-ad43-4751-b1a5-9084aca69676"), "Rejected", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "الطلب لم يتم قبوله لأسباب وظيفية أو تنظيمية.", "The application was not accepted for functional or organizational reasons.", 7, true, false, "مرفوض", "Rejected", null, null },
                    { new Guid("bced01d8-3784-4e2c-9312-930951cc59d8"), "Cancelled", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "المرشح قام بإلغاء الطلب أو تم إلغاؤه وفق الإجراءات.", "The candidate cancelled the application or it was cancelled procedurally.", 8, true, false, "ملغي", "Cancelled", null, null },
                    { new Guid("ca054592-8617-406d-8e8f-3a773b3d0d5e"), "Closed", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "الوظيفة انتهت أو أُغلقت من قبل الموارد البشرية ولا يمكن اتخاذ أي إجراء عليها.", "The job has ended or was closed by HR and no further action can be taken.", 9, true, false, "مغلق", "Closed", null, null },
                    { new Guid("f0bc801d-f54c-4a0e-8aae-00694e4fc80d"), "NewInvitation", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "وظيفة تمت دعوة المرشح لها ولم يقم بقراءتها أو فتحها بعد.", "The candidate was invited but has not opened or viewed it yet.", 1, true, false, "دعوة جديدة", "New Invitation", null, null }
                });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "JobCategory",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsActive", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("3d31e01b-9c57-4b47-8584-9fa8949838e8"), "Labor", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "فئة الوظائف العمالية", "Category for labor jobs", 0, true, false, "عمالي", "Labor", null, null },
                    { new Guid("4e7c8fe7-475b-4e4d-99f0-cfef85020d5b"), "Administrative", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "فئة الوظائف الإدارية", "Category for administrative jobs", 0, true, false, "إداري", "Administrative", null, null },
                    { new Guid("62ecfbcc-7ae7-45c0-9db4-fd50751a336e"), "Academic", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "فئة الوظائف الأكاديمية", "Category for academic jobs", 0, true, false, "أكاديمي", "Academic", null, null }
                });

            migrationBuilder.InsertData(
                schema: "hr",
                table: "JobCategoryCandidateSettings",
                columns: new[] { "Id", "AcademicJobVacancies", "AdministrativeJobVacancies", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "IsDeleted", "LaborJobVacancies", "UpdatedById", "UpdatedDate" },
                values: new object[] { new Guid("f1f1ce2e-1f0b-42b9-9a52-7996fd4b5c29"), 5, 1, null, new DateTimeOffset(new DateTime(1900, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 3, 0, 0, 0)), null, null, false, 1, null, null });

            migrationBuilder.InsertData(
                schema: "hr",
                table: "JobPointConfiguration",
                columns: new[] { "Id", "ApplicantCategoryMaxPoints", "CertificatesMaxPoints", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "EducationMaxPoints", "ExperienceMaxPoints", "IsDeleted", "LanguagesMaxPoints", "MaxPoints", "SkillsMaxPoints", "TrainingMaxPoints", "UpdatedById", "UpdatedDate" },
                values: new object[] { new Guid("4b39e8ae-991a-4a59-b0a6-0fc17a59faf8"), 250, 100, null, new DateTimeOffset(new DateTime(1900, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 3, 0, 0, 0)), null, null, 100, 100, false, 100, 1000, 250, 100, null, null });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "JobStatus",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsActive", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("0d21e063-48d3-d320-4078-d85a7c2bf622"), "ReadyForAnnouncement", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "الوظيفة جاهزة للإعلان.", "Job is ready to be announced.", 7, true, false, "جاهزة للإعلان", "Ready For Announcement", null, null },
                    { new Guid("114dae76-bde9-3efa-2a2b-803ebd92e109"), "Closed", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "تم الوصول إلى تاريخ نهاية التقديم ولن يُسمح بإرسال الطلبات.", "Application end date has been reached; job is not accepting new applications.", 3, true, false, "متوقفة", "Closed", null, null },
                    { new Guid("1e3ecad5-63fa-a11c-7acb-dd4c62ef74fd"), "Published", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "تم نشر الوظيفة.", "Job is published.", 8, true, false, "منشورة", "Published", null, null },
                    { new Guid("5c360b07-157c-630a-254a-9c01587d80a8"), "Approved", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "تم اعتماد الوظيفة.", "Job was approved.", 6, true, false, "معتمدة", "Approved", null, null },
                    { new Guid("c0d95787-8505-b84f-6f50-0b461ece500d"), "Cancelled", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "تم إلغاء الوظيفة ككل ولن يتم نشرها على المنصة.", "The job has been cancelled and will not be published on the platform.", 4, true, false, "ملغية", "Cancelled", null, null },
                    { new Guid("c64916a6-4bd2-a4b6-afbe-c5c3b4926530"), "Draft", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "تم إنشاء الوظيفة كمسودة ولم يتم إرسالها للجمهور.", "Job is created as a draft and not visible to the public.", 1, true, false, "مسودة", "Draft", null, null },
                    { new Guid("e07bd71f-c466-0eea-49cc-2b13d1d9403f"), "PendingApproval", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "الوظيفة قيد الأعتماد.", "Job is waiting for approval.", 5, true, false, "قيد الأعتماد", "Pending Approval", null, null },
                    { new Guid("e0cd7b22-8948-0c37-9b15-2e5217f00065"), "NeedUpdate", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "الوظيفة تحتاج للتعديل", "Job needs to be updated.", 1, true, false, "تحتاج للتعديل", "Need Update", null, null },
                    { new Guid("e0cd7b22-8948-0c37-9b15-2e5217f0c565"), "Active", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "الوظيفة جاهزة للتقديم ويمكن إرسالها للجمهور المطلوب.", "Job is open for applications and can be published to the target audience.", 2, true, false, "نشطة", "Active", null, null },
                    { new Guid("f2e7748a-12fe-53ac-fbdf-e989f8aa498a"), "Rejected", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "تم رفض الوظيفة.", "Job was rejected.", 9, true, false, "مرفوضة", "Rejected", null, null }
                });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "Language",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsActive", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("8672c4c2-f635-4d26-843a-223fba3a6322"), "English", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 2, true, false, "الإنجليزية", "English", null, null },
                    { new Guid("9843b692-ef7c-44a7-b1e8-1dcf2b6d87dd"), "Arabic", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 1, true, false, "العربية", "Arabic", null, null }
                });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "LanguageLevel",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsActive", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("5161f23a-f501-414c-b4fe-81cde9ebcd5d"), "Expert", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 4, true, false, "خبير", "Expert", null, null },
                    { new Guid("9aa2c1bc-2040-40e1-86a2-72cac90928e1"), "Advanced", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 3, true, false, "متقدم", "Advanced", null, null },
                    { new Guid("afd6d7b4-9e20-40bc-a571-0df1670761b0"), "Intermediate", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 2, true, false, "متوسط", "Intermediate", null, null },
                    { new Guid("c7d8b159-c9dc-48a1-be6d-26493423f8a9"), "Native", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 4, true, false, "لغة أم", "Native", null, null },
                    { new Guid("f0c9e84a-258f-4f6b-a469-153a92d68730"), "Basic", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 1, true, false, "مبتدئ", "Basic", null, null }
                });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "MaritalStatus",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsActive", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("18e83653-978c-44c8-8691-43f95d9a5b7d"), "Divorced", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 3, true, false, "مطلق", "Divorced", null, null },
                    { new Guid("7113eb36-44b8-457a-96e9-61bfe6a06f05"), "Married", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 2, true, false, "متزوج", "Married", null, null },
                    { new Guid("8f22e74f-672b-47f4-8f32-93f9dbcb15da"), "Widowed", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 4, true, false, "أرمل", "Widowed", null, null },
                    { new Guid("c28ab4a0-59b0-4a64-82c4-ba1bd89efaba"), "Single", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 1, true, false, "أعزب", "Single", null, null }
                });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "Permission",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsActive", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("0148d1fe-79a7-0d5d-8391-7baf026f65fd"), "jobs.points.view", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 33, true, false, "نقاط الوظائف - عرض", "Jobs Points - View", null, null },
                    { new Guid("06a9ef54-2bd0-8b55-b746-16d7bce6274c"), "languages.view", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 8, true, false, "اللغات - عرض", "Languages - View", null, null },
                    { new Guid("116b0925-93cf-6157-ba24-c12a87667dae"), "jobs.approve", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 32, true, false, "الوظائف - اعتماد", "Jobs - Approve", null, null },
                    { new Guid("1502d704-7619-0255-ade1-8eab23a079a3"), "languages.manage", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 9, true, false, "اللغات - إدارة", "Languages - Manage", null, null },
                    { new Guid("1e01a537-dae3-a85b-942c-0cf3541a2196"), "profile.distribution.view", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 22, true, false, "توزيع الملفات - عرض", "Profile Distribution - View", null, null },
                    { new Guid("241a2e7e-4868-3e57-9f4b-a2e1d3bd2423"), "universities.manage", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 15, true, false, "الجامعات - إدارة", "Universities - Manage", null, null },
                    { new Guid("27a99574-bb73-425f-a1b3-0853b5234580"), "users.manage", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 5, true, false, "المستخدمون - إدارة", "Users - Manage", null, null },
                    { new Guid("35dea045-9dab-125d-ab51-c820bf59525a"), "universities.view", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 14, true, false, "الجامعات - عرض", "Universities - View", null, null },
                    { new Guid("377a116d-4709-d65d-9565-c01e8908d25a"), "jobs.points.manage", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 33, true, false, "نقاط الوظائف - إدارة", "Jobs Points - Manage", null, null },
                    { new Guid("3b9b386e-13a1-f959-a87e-a9d598b7ecf8"), "targetentities.manage", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 17, true, false, "الجهات المستهدفة - إدارة", "Target Entities - Manage", null, null },
                    { new Guid("42ba2b73-02d4-e156-803a-bfed80608058"), "office.users.manage", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 61, true, false, "مستخدمو المكتب - إدارة", "Office Users - Manage", null, null },
                    { new Guid("42ea0acc-9bac-b15a-aef3-2ca7c7b9dbed"), "profile.logs.view", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 14, true, false, "سجل الملفات - عرض", "Profile Logs - View", null, null },
                    { new Guid("48571726-6c3c-0d55-9e84-5827bb52c9a4"), "candidate.users.view", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 18, true, false, "مستخدمو المرشحين - عرض", "Candidate Users - View", null, null },
                    { new Guid("49bcd54d-251e-b557-9d0f-67f8de39c326"), "jobs.view", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 30, true, false, "الوظائف - عرض", "Jobs - View", null, null },
                    { new Guid("5adc185c-aabf-a55f-9675-769a482552da"), "religions.view", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 10, true, false, "الديانات - عرض", "Religions - View", null, null },
                    { new Guid("5b3122dc-edce-e253-a406-f4f384f3d20b"), "organization-structures.manage", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 80, true, false, "الهياكل التنظيمية - إدارة", "Organization Structures - Manage", null, null },
                    { new Guid("5cffb2e6-4548-0f5e-a045-79765647de0a"), "profile.approval.view", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 24, true, false, "اعتماد الملفات - عرض", "Profile Approval - View", null, null },
                    { new Guid("5d68fab4-cd5e-1958-a5f0-19d93f0db665"), "nominations.manage", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 41, true, false, "الترشيحات - إدارة", "Nominations - Manage", null, null },
                    { new Guid("60271cb4-38b6-f752-98a3-a62a493f3486"), "major-skill.management", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 70, true, false, "مهارات التخصص - إدارة", "Major Skills - Manage", null, null },
                    { new Guid("64270af1-5bd1-9351-b23f-a993bc4ee8ed"), "profile.approval.review", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 25, true, false, "اعتماد الملفات - مراجعة", "Profile Approval - Review", null, null },
                    { new Guid("64b8bb0d-a959-9555-ad77-3c3ab94bd7a6"), "nominations.view", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 40, true, false, "الترشيحات - عرض", "Nominations - View", null, null },
                    { new Guid("6748978c-f5d7-7155-bba0-d992051bfa0f"), "roles.view", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 2, true, false, "الأدوار - عرض", "Roles - View", null, null },
                    { new Guid("6a437b3a-a459-c659-bf8c-f8035a7fed65"), "profile.approval.changes", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 26, true, false, "اعتماد الملفات - تعديلات", "Profile Approval - Changes", null, null },
                    { new Guid("73bccdc8-d7e8-8d58-b71e-ce853d1cbca7"), "office.users.view", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 60, true, false, "مستخدمو المكتب - عرض", "Office Users - View", null, null },
                    { new Guid("829e83f6-214b-e755-b6e8-01ccc773d5fe"), "offices.manage", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 7, true, false, "المكاتب - إدارة", "Offices - Manage", null, null },
                    { new Guid("83ff0b70-5423-b05f-a5aa-dd69f5155d6c"), "candidate.users.manage", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 19, true, false, "مستخدمو المرشحين - إدارة", "Candidate Users - Manage", null, null },
                    { new Guid("931e09d8-30f2-455a-b832-aad017c97cf8"), "offices.view", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 6, true, false, "المكاتب - عرض", "Offices - View", null, null },
                    { new Guid("9539bc9c-9df3-355c-870f-1664738a9c83"), "users.view", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 4, true, false, "المستخدمون - عرض", "Users - View", null, null },
                    { new Guid("a2d2c7dc-d2a2-fd51-af47-34a24b77a14b"), "countries.view", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 12, true, false, "الدول - عرض", "Countries - View", null, null },
                    { new Guid("a601244f-898e-f95c-9e8b-a2ff34797f75"), "targetentities.view", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 16, true, false, "الجهات المستهدفة - عرض", "Target Entities - View", null, null },
                    { new Guid("a6f1dfe0-8158-a65a-af51-860dc4f2b853"), "dashboard.view", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 1, true, false, "لوحة التحكم - عرض", "Dashboard - View", null, null },
                    { new Guid("a8a3c689-062d-a457-ab7f-56a89ce5ba37"), "kawader.manage", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 50, true, false, "الكوادر - إدارة", "Kawader - Manage", null, null },
                    { new Guid("ab24906c-15de-7251-bbc0-d278fda72ae0"), "jobs.manage", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 31, true, false, "الوظائف - إدارة", "Jobs - Manage", null, null },
                    { new Guid("b99f006a-c1ca-e556-b8cc-c9608c643306"), "roles.manage", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 3, true, false, "الأدوار - إدارة", "Roles - Manage", null, null },
                    { new Guid("ca31d283-6388-325a-8dd6-9b26844c45fa"), "profile.distribution.manage", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 23, true, false, "توزيع الملفات - إدارة", "Profile Distribution - Manage", null, null },
                    { new Guid("e8075b8b-8c77-ae57-a91e-530d5bdc07a9"), "profile.view", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 20, true, false, "الملف الشخصي - عرض", "Profile - View", null, null },
                    { new Guid("e95eaa09-b74d-1955-b325-8896bc574523"), "jobs.points.approve", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 33, true, false, "نقاط الوظائف - إعتماد", "Jobs Points - Approve", null, null },
                    { new Guid("ec49162a-0178-965a-807a-7752d369bbc2"), "profile.manage", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 21, true, false, "الملف الشخصي - إدارة", "Profile - Manage", null, null },
                    { new Guid("f523d40b-2395-e454-a2d3-22e96edf347d"), "religions.manage", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 11, true, false, "الديانات - إدارة", "Religions - Manage", null, null },
                    { new Guid("fe4eb16e-25b4-7950-be57-a79c2c212fa3"), "jobs.invitations.view", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 34, true, false, "دعوات الوظائف - عرض", "Jobs Invitations - View", null, null },
                    { new Guid("ffd0887b-67c6-a35f-82e8-1a973da7448a"), "countries.manage", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 13, true, false, "الدول - إدارة", "Countries - Manage", null, null }
                });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "ProviderLogin",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsActive", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("0d1ab8a4-2b89-4dcc-aa6f-6ec92ccb887c"), "Google", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 1, true, false, "جوجل", "Google", null, null },
                    { new Guid("b8854959-1e46-4595-b51f-de3c09e3ed85"), "QatarPass", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 2, true, false, "قطر باس", "QatarPass", null, null },
                    { new Guid("e0575116-ea2b-4917-965f-214048a4c78b"), "AzureAD", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 3, true, false, "أزور", "AzureAD", null, null },
                    { new Guid("fc379bb9-39f7-458a-85f8-6e54d1780178"), "QatarResidentOtp", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 4, true, false, "قطر OTP", "QatarResidentOtp", null, null }
                });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "RatingGrade",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsActive", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("08e6f782-458e-4334-9bf1-f599c53b437a"), "Acceptable", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 5, true, false, "مقبول", "Acceptable", null, null },
                    { new Guid("2cf3d4e2-33cd-4671-9667-1b5e6e0cee9a"), "VeryGood", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 3, true, false, "جيد جدا", "Very Good", null, null },
                    { new Guid("4dbd3381-f57a-4f6c-a718-432970d32276"), "AboveExcellent", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 1, true, false, "امتياز", "Above Excellent", null, null },
                    { new Guid("71a78988-825a-4a0f-94a3-f59089dbe33d"), "Good", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 4, true, false, "جيد", "Good", null, null },
                    { new Guid("76bd9c52-9c81-4d8f-afb4-fdd924744082"), "Excellent", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 2, true, false, "ممتاز", "Excellent", null, null }
                });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "Religion",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsActive", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("185cbc18-2a59-40b0-a8d9-64b451f91ddf"), "Other", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 6, true, false, "أخرى", "Other", null, null },
                    { new Guid("51588ec8-2d50-4365-aa8c-84efaec02e09"), "Christian", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 2, true, false, "المسيحية", "Christianity", null, null },
                    { new Guid("5970a291-e636-4fd4-ab27-2af22dd77e9a"), "Islam", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 1, true, false, "الإسلام", "Islam", null, null },
                    { new Guid("6af60f76-d289-42d1-a3de-a3fa89056678"), "Sikh", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 5, true, false, "السيخية", "Sikhism", null, null },
                    { new Guid("7d76cd4c-915a-4ce2-a2b0-decddf0f7490"), "Hindu", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 3, true, false, "الهندوسية", "Hinduism", null, null },
                    { new Guid("c419dbdf-d0fc-4555-af2a-c5ed46847f8f"), "Buddhist", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 4, true, false, "البوذية", "Buddhism", null, null }
                });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "Sector",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsActive", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("a3c9f0eb-5d6e-6c4f-0a7b-8c9d0e1a2b3c"), "PrivateEducationSector", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "قطاع التعليم الخاص", "Private Education Sector", 5, true, false, "التعليم الخاص", "Private Education Sector", null, null },
                    { new Guid("b4da01fc-6e7f-7d50-1b8c-9d0e1a2b3c4d"), "AssessmentSector", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "قطاع التقييم", "Assessment Sector", 6, true, false, "التقييم", "Assessment Sector", null, null },
                    { new Guid("c5eb12fd-7f80-8e61-2c9d-0e1a2b3c4d5e"), "SharedServicesSector", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "قطاع الخدمات المشتركة", "Shared Services Sector", 7, true, false, "الخدمات المشتركة", "Shared Services Sector", null, null },
                    { new Guid("e1a7f8d9-3b4c-4a2d-8e5f-6a7b8c9d0e1f"), "DeputyMinisterSector", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "قطاع سعادة الوكيل", "Deputy Minister Sector", 3, true, false, "سعادة الوكيل", "Deputy Minister Sector", null, null },
                    { new Guid("f2b8e9fa-4c5d-5b3e-9f6a-7b8c9d0e1a2b"), "GeneralEducationSector", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "قطاع التعليم العام", "General Education Sector", 4, true, false, "التعليم العام", "General Education Sector", null, null }
                });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "SkillLevel",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsActive", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("2cf3d4e2-33cd-4671-9667-1b5e6e0cee9a"), "Intermediate", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 3, true, false, "متوسط", "Intermediate", null, null },
                    { new Guid("4dbd3381-f57a-4f6c-a718-432970d32276"), "Expert", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 1, true, false, "خبير", "Expert", null, null },
                    { new Guid("69c0943a-f04f-4b6c-9145-1fbfae4b5c2e"), "Basic", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 4, true, false, "أساسي", "Basic", null, null },
                    { new Guid("b8f10518-bf02-4357-a0e3-1f6bfb1746cd"), "Advanced", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 2, true, false, "متقدم", "Advanced", null, null }
                });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "SkillType",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsActive", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("2f1b6ce7-cbc3-2b5c-b264-a02c6a87be1d"), "Educational", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "مهارات تعليمية تم الحصول عليها من خلال التعليم الرسمي", "Educational skills acquired through formal education", 1, true, false, "تعليمي", "Educational", null, null },
                    { new Guid("83b504d0-25c1-5ca0-6757-df299869f002"), "Professional", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "مهارات مهنية ناعمة وكفاءات مكان العمل", "Professional soft skills and workplace competencies", 3, true, false, "مهني", "Professional", null, null },
                    { new Guid("d14ac141-c057-16f5-ddd8-97d02f6a7c9b"), "Technical", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "مهارات تقنية أو صلبة تتعلق بأدوات أو تقنيات أو منهجيات محددة", "Technical or hard skills related to specific tools, technologies, or methodologies", 2, true, false, "تقني", "Technical", null, null },
                    { new Guid("f91eb9e6-7a3f-76d6-1fe1-4443cecc5b9a"), "Other", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "أنواع أخرى من المهارات غير المصنفة أعلاه", "Other types of skills not categorized above", 4, true, false, "أخرى", "Other", null, null }
                });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "SponsorType",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsActive", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("51588ec8-2d50-4365-aa8c-84efaec02e09"), "Company", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 2, true, false, "منشأة", "Company", null, null },
                    { new Guid("5970a291-e636-4fd4-ab27-2af22dd77e9a"), "Individual", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 1, true, false, "فرد", "Individual", null, null }
                });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "StudyType",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsActive", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("0a09774f-fa61-4896-b808-c8576cd0bf6b"), "DistanceLearning", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 2, true, false, "تعليم عن بعد", "Distance Learning", null, null },
                    { new Guid("29754e1d-9125-4582-a998-68a72b8f8443"), "Affiliation", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 3, true, false, "دراسة انتساب", "Affiliation Study", null, null },
                    { new Guid("b28f6a3a-e3ec-401e-9bc7-c4156b4bf76f"), "Regular", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 1, true, false, "دراسة نظامية", "Regular Study", null, null }
                });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "TargetEntity",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsActive", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("1548322c-6c05-4754-a89c-19e9d2443d62"), "Schools", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 1, true, false, "المدارس", "Schools", null, null },
                    { new Guid("8fadf8df-ae7e-4d22-8cb8-f79ed9b14375"), "Ministry", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 2, true, false, "الوزارة", "Ministry", null, null }
                });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "UserType",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsActive", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("5d1970fe-8398-44c0-88ea-560521933912"), "OfficeUser", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 4, true, false, "موظف مكتب", "OfficeUser", null, null },
                    { new Guid("a1b2c3d4-e5f6-4879-8a3b-5c6d7e8f9a0b"), "Employee", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 1, true, false, "موظف", "Employee", null, null },
                    { new Guid("b2c3d4e5-f6a7-5984-9b2c-6d7e8f9a0b1c"), "Applicant", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 2, true, false, "متقدم", "Applicant", null, null },
                    { new Guid("c3d4e5f6-a7b8-6a95-0c3d-7e8f9a0b1c2d"), "Admin", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 3, true, false, "مسؤول النظام", "Administrator", null, null }
                });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "WorkType",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsActive", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("0fdfadbf-e5a0-41d3-a74d-d86354fed434"), "FullTime", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "وظيفة بدوام كامل", "Full-time job", 1, true, false, "دوام كامل", "Full-Time", null, null },
                    { new Guid("e30074cd-52c6-41b5-a692-57626d109c73"), "PartTime", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "وظيفة بدوام جزئي", "Part-time job", 2, true, false, "دوام جزئي", "Part-Time", null, null }
                });

            migrationBuilder.InsertData(
                table: "AspNetRoleClaims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "RoleId" },
                values: new object[,]
                {
                    { -77, "permission", "major-skill.management", new Guid("d8689e0c-d872-42e9-87b7-c3bb27305e07") },
                    { -76, "permission", "organization-structures.manage", new Guid("d8689e0c-d872-42e9-87b7-c3bb27305e07") },
                    { -75, "permission", "candidate.users.manage", new Guid("d8689e0c-d872-42e9-87b7-c3bb27305e07") },
                    { -74, "permission", "candidate.users.view", new Guid("d8689e0c-d872-42e9-87b7-c3bb27305e07") },
                    { -73, "permission", "office.users.manage", new Guid("d8689e0c-d872-42e9-87b7-c3bb27305e07") },
                    { -72, "permission", "office.users.view", new Guid("d8689e0c-d872-42e9-87b7-c3bb27305e07") },
                    { -71, "permission", "kawader.manage", new Guid("d8689e0c-d872-42e9-87b7-c3bb27305e07") },
                    { -70, "permission", "nominations.manage", new Guid("d8689e0c-d872-42e9-87b7-c3bb27305e07") },
                    { -69, "permission", "nominations.view", new Guid("d8689e0c-d872-42e9-87b7-c3bb27305e07") },
                    { -68, "permission", "jobs.invitations.view", new Guid("d8689e0c-d872-42e9-87b7-c3bb27305e07") },
                    { -67, "permission", "jobs.points.approve", new Guid("d8689e0c-d872-42e9-87b7-c3bb27305e07") },
                    { -66, "permission", "jobs.points.view", new Guid("d8689e0c-d872-42e9-87b7-c3bb27305e07") },
                    { -65, "permission", "jobs.points.manage", new Guid("d8689e0c-d872-42e9-87b7-c3bb27305e07") },
                    { -64, "permission", "jobs.approve", new Guid("d8689e0c-d872-42e9-87b7-c3bb27305e07") },
                    { -63, "permission", "jobs.manage", new Guid("d8689e0c-d872-42e9-87b7-c3bb27305e07") },
                    { -62, "permission", "jobs.view", new Guid("d8689e0c-d872-42e9-87b7-c3bb27305e07") },
                    { -61, "permission", "profile.approval.changes", new Guid("d8689e0c-d872-42e9-87b7-c3bb27305e07") },
                    { -60, "permission", "profile.approval.review", new Guid("d8689e0c-d872-42e9-87b7-c3bb27305e07") },
                    { -59, "permission", "profile.approval.view", new Guid("d8689e0c-d872-42e9-87b7-c3bb27305e07") },
                    { -58, "permission", "profile.distribution.manage", new Guid("d8689e0c-d872-42e9-87b7-c3bb27305e07") },
                    { -57, "permission", "profile.distribution.view", new Guid("d8689e0c-d872-42e9-87b7-c3bb27305e07") },
                    { -56, "permission", "profile.manage", new Guid("d8689e0c-d872-42e9-87b7-c3bb27305e07") },
                    { -55, "permission", "profile.view", new Guid("d8689e0c-d872-42e9-87b7-c3bb27305e07") },
                    { -54, "permission", "dashboard.view", new Guid("d8689e0c-d872-42e9-87b7-c3bb27305e07") },
                    { -53, "permission", "profile.approval.changes", new Guid("12f5805d-6970-4a9e-a275-2b7cf3db3bb8") },
                    { -52, "permission", "profile.approval.review", new Guid("12f5805d-6970-4a9e-a275-2b7cf3db3bb8") },
                    { -51, "permission", "profile.approval.view", new Guid("12f5805d-6970-4a9e-a275-2b7cf3db3bb8") },
                    { -50, "permission", "dashboard.view", new Guid("12f5805d-6970-4a9e-a275-2b7cf3db3bb8") },
                    { -49, "permission", "profile.distribution.manage", new Guid("98e20970-b6bc-4da9-a947-f75e9adae3ca") },
                    { -48, "permission", "profile.distribution.view", new Guid("98e20970-b6bc-4da9-a947-f75e9adae3ca") },
                    { -47, "permission", "office.users.manage", new Guid("98e20970-b6bc-4da9-a947-f75e9adae3ca") },
                    { -46, "permission", "office.users.view", new Guid("98e20970-b6bc-4da9-a947-f75e9adae3ca") },
                    { -45, "permission", "dashboard.view", new Guid("98e20970-b6bc-4da9-a947-f75e9adae3ca") },
                    { -44, "permission", "dashboard.view", new Guid("5f12e420-f666-4af4-a8fa-4e4aa755fdcd") },
                    { -43, "permission", "dashboard.view", new Guid("ac011a30-6b0e-496c-a8ef-ba8132bd1808") },
                    { -42, "permission", "dashboard.view", new Guid("5b757ede-93e1-4c7a-88d3-5e89007d648b") },
                    { -41, "permission", "organization-structures.manage", new Guid("1361d691-53c5-4a84-aea1-64ff134cf082") },
                    { -40, "permission", "major-skill.management", new Guid("1361d691-53c5-4a84-aea1-64ff134cf082") },
                    { -39, "permission", "office.users.manage", new Guid("1361d691-53c5-4a84-aea1-64ff134cf082") },
                    { -38, "permission", "office.users.view", new Guid("1361d691-53c5-4a84-aea1-64ff134cf082") },
                    { -37, "permission", "kawader.manage", new Guid("1361d691-53c5-4a84-aea1-64ff134cf082") },
                    { -36, "permission", "nominations.manage", new Guid("1361d691-53c5-4a84-aea1-64ff134cf082") },
                    { -35, "permission", "nominations.view", new Guid("1361d691-53c5-4a84-aea1-64ff134cf082") },
                    { -34, "permission", "jobs.invitations.view", new Guid("1361d691-53c5-4a84-aea1-64ff134cf082") },
                    { -33, "permission", "jobs.points.approve", new Guid("1361d691-53c5-4a84-aea1-64ff134cf082") },
                    { -32, "permission", "jobs.points.view", new Guid("1361d691-53c5-4a84-aea1-64ff134cf082") },
                    { -31, "permission", "jobs.points.manage", new Guid("1361d691-53c5-4a84-aea1-64ff134cf082") },
                    { -30, "permission", "jobs.approve", new Guid("1361d691-53c5-4a84-aea1-64ff134cf082") },
                    { -29, "permission", "jobs.manage", new Guid("1361d691-53c5-4a84-aea1-64ff134cf082") },
                    { -28, "permission", "jobs.view", new Guid("1361d691-53c5-4a84-aea1-64ff134cf082") },
                    { -27, "permission", "profile.approval.changes", new Guid("1361d691-53c5-4a84-aea1-64ff134cf082") },
                    { -26, "permission", "profile.approval.review", new Guid("1361d691-53c5-4a84-aea1-64ff134cf082") },
                    { -25, "permission", "profile.approval.view", new Guid("1361d691-53c5-4a84-aea1-64ff134cf082") },
                    { -24, "permission", "profile.distribution.manage", new Guid("1361d691-53c5-4a84-aea1-64ff134cf082") },
                    { -23, "permission", "profile.distribution.view", new Guid("1361d691-53c5-4a84-aea1-64ff134cf082") },
                    { -22, "permission", "profile.manage", new Guid("1361d691-53c5-4a84-aea1-64ff134cf082") },
                    { -21, "permission", "profile.view", new Guid("1361d691-53c5-4a84-aea1-64ff134cf082") },
                    { -20, "permission", "profile.logs.view", new Guid("1361d691-53c5-4a84-aea1-64ff134cf082") },
                    { -19, "permission", "targetentities.manage", new Guid("1361d691-53c5-4a84-aea1-64ff134cf082") },
                    { -18, "permission", "targetentities.view", new Guid("1361d691-53c5-4a84-aea1-64ff134cf082") },
                    { -17, "permission", "universities.manage", new Guid("1361d691-53c5-4a84-aea1-64ff134cf082") },
                    { -16, "permission", "universities.view", new Guid("1361d691-53c5-4a84-aea1-64ff134cf082") },
                    { -15, "permission", "countries.manage", new Guid("1361d691-53c5-4a84-aea1-64ff134cf082") },
                    { -14, "permission", "countries.view", new Guid("1361d691-53c5-4a84-aea1-64ff134cf082") },
                    { -13, "permission", "religions.manage", new Guid("1361d691-53c5-4a84-aea1-64ff134cf082") },
                    { -12, "permission", "religions.view", new Guid("1361d691-53c5-4a84-aea1-64ff134cf082") },
                    { -11, "permission", "languages.manage", new Guid("1361d691-53c5-4a84-aea1-64ff134cf082") },
                    { -10, "permission", "languages.view", new Guid("1361d691-53c5-4a84-aea1-64ff134cf082") },
                    { -9, "permission", "offices.manage", new Guid("1361d691-53c5-4a84-aea1-64ff134cf082") },
                    { -8, "permission", "offices.view", new Guid("1361d691-53c5-4a84-aea1-64ff134cf082") },
                    { -7, "permission", "candidate.users.manage", new Guid("1361d691-53c5-4a84-aea1-64ff134cf082") },
                    { -6, "permission", "candidate.users.view", new Guid("1361d691-53c5-4a84-aea1-64ff134cf082") },
                    { -5, "permission", "users.manage", new Guid("1361d691-53c5-4a84-aea1-64ff134cf082") },
                    { -4, "permission", "users.view", new Guid("1361d691-53c5-4a84-aea1-64ff134cf082") },
                    { -3, "permission", "roles.manage", new Guid("1361d691-53c5-4a84-aea1-64ff134cf082") },
                    { -2, "permission", "roles.view", new Guid("1361d691-53c5-4a84-aea1-64ff134cf082") },
                    { -1, "permission", "dashboard.view", new Guid("1361d691-53c5-4a84-aea1-64ff134cf082") }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "Avatar", "ConcurrencyStamp", "CreatedById", "CreatedDate", "CurrentAuthToken", "DeletedById", "DeletedDate", "Discriminator", "Email", "EmailConfirmed", "EmployeeProfileId", "FullNameAr", "FullNameEn", "IsBlocked", "IsDeleted", "LastLoginDate", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "OtpAttempts", "OtpExpiry", "OtpLockedUntilUtc", "OtpReference", "OtpSendWindowStartUtc", "OtpSendsInWindow", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UpdatedById", "UpdatedDate", "UserName", "UserTypeId" },
                values: new object[,]
                {
                    { new Guid("0593ad82-e44e-4f55-aa08-c5c80764a873"), 0, null, "75a677a7-c93d-4940-8666-4d648343104c", null, new DateTimeOffset(new DateTime(1900, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 3, 0, 0, 0)), null, null, null, "EmployeeUser", "t-m.khatatbeh@edu.gov.qa", true, null, "t-m.khatatbeh", "t-m.khatatbeh", false, false, null, false, null, "T-M.KHATATBEH@EDU.GOV.QA", "T-M.KHATATBEH@EDU.GOV.QA", 0, null, null, null, null, 0, null, null, false, "a984b6f5-e904-44b0-8d0d-5e93c07b1510", false, null, null, "t-m.khatatbeh@edu.gov.qa", new Guid("a1b2c3d4-e5f6-4879-8a3b-5c6d7e8f9a0b") },
                    { new Guid("200c5018-fa8c-4ee7-a088-9077200b125c"), 0, null, "75a677a7-c93d-4940-8666-4d648343104c", null, new DateTimeOffset(new DateTime(1900, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 3, 0, 0, 0)), null, null, null, "EmployeeUser", "t-m.abdin@edu.gov.qa", true, null, "t-m.abdin", "t-m.abdin", false, false, null, false, null, "T-M.ABDIN@EDU.GOV.QA", "T-M.ABDIN@EDU.GOV.QA", 0, null, null, null, null, 0, null, null, false, "a984b6f5-e904-44b0-8d0d-5e93c07b1510", false, null, null, "t-m.abdin@edu.gov.qa", new Guid("a1b2c3d4-e5f6-4879-8a3b-5c6d7e8f9a0b") }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "Avatar", "ConcurrencyStamp", "CreatedById", "CreatedDate", "CurrentAuthToken", "DeletedById", "DeletedDate", "Discriminator", "Email", "EmailConfirmed", "FullNameAr", "FullNameEn", "IsBlocked", "IsDeleted", "LastLoginDate", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "OtpAttempts", "OtpExpiry", "OtpLockedUntilUtc", "OtpReference", "OtpSendWindowStartUtc", "OtpSendsInWindow", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UpdatedById", "UpdatedDate", "UserName", "UserTypeId" },
                values: new object[] { new Guid("26058720-5808-435a-abbf-d7a4e751f51e"), 0, null, "75a677a7-c93d-4940-8666-4d648343104c", null, new DateTimeOffset(new DateTime(1900, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 3, 0, 0, 0)), null, null, null, "AdminUser", "t-m.abdin-dev@edu.gov.qa", true, "t-m.abdin-dev", "t-m.abdin-dev", false, false, null, false, null, "T-M.ABDIN-DEV@EDU.GOV.QA", "T-M.ABDIN-DEV@EDU.GOV.QA", 0, null, null, null, null, 0, null, null, false, "a984b6f5-e904-44b0-8d0d-5e93c07b1510", false, null, null, "t-m.abdin-dev@edu.gov.qa", new Guid("c3d4e5f6-a7b8-6a95-0c3d-7e8f9a0b1c2d") });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "Avatar", "ConcurrencyStamp", "CreatedById", "CreatedDate", "CurrentAuthToken", "DeletedById", "DeletedDate", "Discriminator", "Email", "EmailConfirmed", "EmployeeProfileId", "FullNameAr", "FullNameEn", "IsBlocked", "IsDeleted", "LastLoginDate", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "OtpAttempts", "OtpExpiry", "OtpLockedUntilUtc", "OtpReference", "OtpSendWindowStartUtc", "OtpSendsInWindow", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UpdatedById", "UpdatedDate", "UserName", "UserTypeId" },
                values: new object[] { new Guid("42e0d563-7603-453c-81b1-6b2325622b40"), 0, null, "75a677a7-c93d-4940-8666-4d648343104c", null, new DateTimeOffset(new DateTime(1900, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 3, 0, 0, 0)), null, null, null, "EmployeeUser", "t-hu.ahmed@edu.gov.qa", true, null, "t-hu.ahmed", "t-hu.ahmed", false, false, null, false, null, "T-HU.AHMED@EDU.GOV.QA", "T-HU.AHMED@EDU.GOV.QA", 0, null, null, null, null, 0, null, null, false, "a984b6f5-e904-44b0-8d0d-5e93c07b1510", false, null, null, "t-hu.ahmed@edu.gov.qa", new Guid("a1b2c3d4-e5f6-4879-8a3b-5c6d7e8f9a0b") });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "Avatar", "ConcurrencyStamp", "CreatedById", "CreatedDate", "CurrentAuthToken", "DeletedById", "DeletedDate", "Discriminator", "Email", "EmailConfirmed", "FullNameAr", "FullNameEn", "IsBlocked", "IsDeleted", "LastLoginDate", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "OtpAttempts", "OtpExpiry", "OtpLockedUntilUtc", "OtpReference", "OtpSendWindowStartUtc", "OtpSendsInWindow", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UpdatedById", "UpdatedDate", "UserName", "UserTypeId" },
                values: new object[,]
                {
                    { new Guid("48f6d7a2-e821-4ab6-81cf-884ed650d2ec"), 0, null, "75a677a7-c93d-4940-8666-4d648343104c", null, new DateTimeOffset(new DateTime(1900, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 3, 0, 0, 0)), null, null, null, "AdminUser", "t-m.khatatbeh-dev@edu.gov.qa", true, "t-m.khatatbeh-dev", "t-m.khatatbeh-dev", false, false, null, false, null, "T-M.KHATATBEH-DEV@EDU.GOV.QA", "T-M.KHATATBEH-DEV@EDU.GOV.QA", 0, null, null, null, null, 0, null, null, false, "a984b6f5-e904-44b0-8d0d-5e93c07b1510", false, null, null, "t-m.khatatbeh-dev@edu.gov.qa", new Guid("c3d4e5f6-a7b8-6a95-0c3d-7e8f9a0b1c2d") },
                    { new Guid("5a4c8063-07a4-43ba-b4a8-8d980a323738"), 0, null, "75a677a7-c93d-4940-8666-4d648343104c", null, new DateTimeOffset(new DateTime(1900, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 3, 0, 0, 0)), null, null, null, "AdminUser", "t-hu.ahmed-dev@edu.gov.qa", true, "t-hu.ahmed-dev", "t-hu.ahmed-dev", false, false, null, false, null, "T-HU.AHMED-DEV@EDU.GOV.QA", "T-HU.AHMED-DEV@EDU.GOV.QA", 0, null, null, null, null, 0, null, null, false, "a984b6f5-e904-44b0-8d0d-5e93c07b1510", false, null, null, "t-hu.ahmed-dev@edu.gov.qa", new Guid("c3d4e5f6-a7b8-6a95-0c3d-7e8f9a0b1c2d") }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "Avatar", "ConcurrencyStamp", "CreatedById", "CreatedDate", "CurrentAuthToken", "DeletedById", "DeletedDate", "Discriminator", "Email", "EmailConfirmed", "EmployeeProfileId", "FullNameAr", "FullNameEn", "IsBlocked", "IsDeleted", "LastLoginDate", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "OtpAttempts", "OtpExpiry", "OtpLockedUntilUtc", "OtpReference", "OtpSendWindowStartUtc", "OtpSendsInWindow", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UpdatedById", "UpdatedDate", "UserName", "UserTypeId" },
                values: new object[] { new Guid("781561c3-0175-4165-80c1-7c6a79130b25"), 0, null, "75a677a7-c93d-4940-8666-4d648343104c", null, new DateTimeOffset(new DateTime(1900, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 3, 0, 0, 0)), null, null, null, "EmployeeUser", "t-a.jaber@edu.gov.qa", true, null, "t-a.jaber", "t-a.jaber", false, false, null, false, null, "T-A.JABER@EDU.GOV.QA", "T-A.JABER@EDU.GOV.QA", 0, null, null, null, null, 0, null, null, false, "a984b6f5-e904-44b0-8d0d-5e93c07b1510", false, null, null, "t-a.jaber@edu.gov.qa", new Guid("a1b2c3d4-e5f6-4879-8a3b-5c6d7e8f9a0b") });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "Avatar", "ConcurrencyStamp", "CreatedById", "CreatedDate", "CurrentAuthToken", "DeletedById", "DeletedDate", "Discriminator", "Email", "EmailConfirmed", "FullNameAr", "FullNameEn", "IsBlocked", "IsDeleted", "LastLoginDate", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "OtpAttempts", "OtpExpiry", "OtpLockedUntilUtc", "OtpReference", "OtpSendWindowStartUtc", "OtpSendsInWindow", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UpdatedById", "UpdatedDate", "UserName", "UserTypeId" },
                values: new object[] { new Guid("a8e0f354-2e23-41e1-9e1b-a1501b72dc4b"), 0, null, "75a677a7-c93d-4940-8666-4d648343104c", null, new DateTimeOffset(new DateTime(1900, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 3, 0, 0, 0)), null, null, null, "AdminUser", "t-a.jaber-dev@edu.gov.qa", true, "t-a.jaber-dev", "t-a.jaber-dev", false, false, null, false, null, "T-A.JABER-DEV@EDU.GOV.QA", "T-A.JABER-DEV@EDU.GOV.QA", 0, null, null, null, null, 0, null, null, false, "a984b6f5-e904-44b0-8d0d-5e93c07b1510", false, null, null, "t-a.jaber-dev@edu.gov.qa", new Guid("c3d4e5f6-a7b8-6a95-0c3d-7e8f9a0b1c2d") });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "CandidateTypeProviderLogin",
                columns: new[] { "CandidateTypeId", "ProviderLoginId" },
                values: new object[,]
                {
                    { new Guid("42b374d1-8032-44d7-95bd-ff5a64abbc95"), new Guid("0d1ab8a4-2b89-4dcc-aa6f-6ec92ccb887c") },
                    { new Guid("50f14c55-d5f0-4bba-930e-ab73b012e6cb"), new Guid("0d1ab8a4-2b89-4dcc-aa6f-6ec92ccb887c") },
                    { new Guid("50f14c55-d5f0-4bba-930e-ab73b012e6cb"), new Guid("b8854959-1e46-4595-b51f-de3c09e3ed85") },
                    { new Guid("50f14c55-d5f0-4bba-930e-ab73b012e6cb"), new Guid("fc379bb9-39f7-458a-85f8-6e54d1780178") },
                    { new Guid("744256ea-4ee0-4a63-b071-8810895a33dc"), new Guid("b8854959-1e46-4595-b51f-de3c09e3ed85") },
                    { new Guid("744256ea-4ee0-4a63-b071-8810895a33dc"), new Guid("fc379bb9-39f7-458a-85f8-6e54d1780178") },
                    { new Guid("7b06bc91-88b3-46ec-b6cc-ef2338864d41"), new Guid("b8854959-1e46-4595-b51f-de3c09e3ed85") },
                    { new Guid("7b06bc91-88b3-46ec-b6cc-ef2338864d41"), new Guid("fc379bb9-39f7-458a-85f8-6e54d1780178") },
                    { new Guid("ce67465e-6fed-4a83-9161-8eba16a79af3"), new Guid("b8854959-1e46-4595-b51f-de3c09e3ed85") },
                    { new Guid("ce67465e-6fed-4a83-9161-8eba16a79af3"), new Guid("fc379bb9-39f7-458a-85f8-6e54d1780178") }
                });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "Management",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsActive", "IsDeleted", "NameAr", "NameEn", "SectorId", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("453aa49d-8f16-a85b-9991-c8355ac8bf00"), "TrainingCenter", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 2, true, false, "مركز التدريب", "Training Center", new Guid("f2b8e9fa-4c5d-5b3e-9f6a-7b8c9d0e1a2b"), null, null },
                    { new Guid("4575068a-4f0d-b6cd-8ea8-be680d8dc992"), "Minister", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, 1, true, false, "الوزير", "Minister", new Guid("e1a7f8d9-3b4c-4a2d-8e5f-6a7b8c9d0e1f"), null, null }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { new Guid("5f12e420-f666-4af4-a8fa-4e4aa755fdcd"), new Guid("0593ad82-e44e-4f55-aa08-c5c80764a873") },
                    { new Guid("d8689e0c-d872-42e9-87b7-c3bb27305e07"), new Guid("0593ad82-e44e-4f55-aa08-c5c80764a873") },
                    { new Guid("5f12e420-f666-4af4-a8fa-4e4aa755fdcd"), new Guid("200c5018-fa8c-4ee7-a088-9077200b125c") },
                    { new Guid("d8689e0c-d872-42e9-87b7-c3bb27305e07"), new Guid("200c5018-fa8c-4ee7-a088-9077200b125c") },
                    { new Guid("1361d691-53c5-4a84-aea1-64ff134cf082"), new Guid("26058720-5808-435a-abbf-d7a4e751f51e") },
                    { new Guid("5f12e420-f666-4af4-a8fa-4e4aa755fdcd"), new Guid("42e0d563-7603-453c-81b1-6b2325622b40") },
                    { new Guid("d8689e0c-d872-42e9-87b7-c3bb27305e07"), new Guid("42e0d563-7603-453c-81b1-6b2325622b40") },
                    { new Guid("1361d691-53c5-4a84-aea1-64ff134cf082"), new Guid("48f6d7a2-e821-4ab6-81cf-884ed650d2ec") },
                    { new Guid("1361d691-53c5-4a84-aea1-64ff134cf082"), new Guid("5a4c8063-07a4-43ba-b4a8-8d980a323738") },
                    { new Guid("5f12e420-f666-4af4-a8fa-4e4aa755fdcd"), new Guid("781561c3-0175-4165-80c1-7c6a79130b25") },
                    { new Guid("d8689e0c-d872-42e9-87b7-c3bb27305e07"), new Guid("781561c3-0175-4165-80c1-7c6a79130b25") },
                    { new Guid("1361d691-53c5-4a84-aea1-64ff134cf082"), new Guid("a8e0f354-2e23-41e1-9e1b-a1501b72dc4b") }
                });

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "Department",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsActive", "IsDeleted", "ManagementId", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("0bfb044d-9f85-4f46-a1eb-920a1f9f519a"), "EarlyChildhoodEducation", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "تعليم الطفولة المبكرة", "Early Childhood Education Department", 6, true, false, new Guid("453aa49d-8f16-a85b-9991-c8355ac8bf00"), "تعليم الطفولة المبكرة", "Early Childhood Education", null, null },
                    { new Guid("1b01eb93-b6e3-447a-8d1c-a9ce59bf5ca7"), "HigherEducation", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "التعليم العالي", "Higher Education Department", 7, true, false, new Guid("453aa49d-8f16-a85b-9991-c8355ac8bf00"), "التعليم العالي", "Higher Education", null, null },
                    { new Guid("2a65e4a6-ca1b-4da3-8375-299ec39a50f9"), "PrimaryEducation", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "التعليم الابتدائي", "Primary Education Department", 8, true, false, new Guid("453aa49d-8f16-a85b-9991-c8355ac8bf00"), "التعليم الابتدائي", "Primary Education", null, null },
                    { new Guid("48d2a180-30dc-4314-b222-92750a9d0afe"), "InformationSystems", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "نظام الاعتماد", "Information Systems Department", 1, true, false, new Guid("4575068a-4f0d-b6cd-8ea8-be680d8dc992"), "نظام الاعتماد", "Information Systems", null, null },
                    { new Guid("6881d9fd-7281-457a-a2e1-3cbe827622e8"), "Evaluation", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "التقويم", "Evaluation Department", 4, true, false, new Guid("453aa49d-8f16-a85b-9991-c8355ac8bf00"), "التقويم", "Evaluation", null, null },
                    { new Guid("7e225d86-c5e5-4225-b9fc-d255a975f5d1"), "HumanResources", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "الموارد البشرية", "Human Resources Department", 2, true, false, new Guid("453aa49d-8f16-a85b-9991-c8355ac8bf00"), "الموارد البشرية", "Human Resources", null, null },
                    { new Guid("8acd1c68-9b65-40e1-8776-ce6d7afcf542"), "CommunicationsMedia", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "الاتصالات والاعلام", "Communications and Media Department", 10, true, false, new Guid("453aa49d-8f16-a85b-9991-c8355ac8bf00"), "الاتصالات والاعلام", "Communications and Media", null, null },
                    { new Guid("8fd80cb6-794a-4211-b8a4-139e8e026e5b"), "AdministrativeFinancialAffairs", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "الشؤون الادارية والمالية", "Administrative and Financial Affairs Department", 3, true, false, new Guid("453aa49d-8f16-a85b-9991-c8355ac8bf00"), "الشؤون الادارية والمالية", "Administrative and Financial Affairs", null, null },
                    { new Guid("91a511fb-9b43-47fd-9cd5-fc2a9ecbc32e"), "SchoolAffairs", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "شؤون المدارس", "School Affairs Department", 9, true, false, new Guid("453aa49d-8f16-a85b-9991-c8355ac8bf00"), "شؤون المدارس", "School Affairs", null, null },
                    { new Guid("b858ce40-a3ac-4d27-aba9-86ef62fab4fe"), "Curriculum", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "المناهج", "Curriculum Department", 5, true, false, new Guid("453aa49d-8f16-a85b-9991-c8355ac8bf00"), "المناهج", "Curriculum", null, null }
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
                name: "IX_AspNetUsers_EmployeeProfileId",
                table: "AspNetUsers",
                column: "EmployeeProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_OfficeId",
                table: "AspNetUsers",
                column: "OfficeId");

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
                name: "IX_EmployeeProfile_CreatedById",
                table: "EmployeeProfile",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeProfile_DeletedById",
                table: "EmployeeProfile",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeProfile_UpdatedById",
                table: "EmployeeProfile",
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
                name: "IX_Invitation_ApplicantId",
                schema: "hr",
                table: "Invitation",
                column: "ApplicantId");

            migrationBuilder.CreateIndex(
                name: "IX_Invitation_CreatedById",
                schema: "hr",
                table: "Invitation",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Invitation_DeletedById",
                schema: "hr",
                table: "Invitation",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_Invitation_InvitationStatusId",
                schema: "hr",
                table: "Invitation",
                column: "InvitationStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Invitation_JobId",
                schema: "hr",
                table: "Invitation",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_Invitation_UpdatedById",
                schema: "hr",
                table: "Invitation",
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
                name: "IX_JobCandidateFilterSetting_CreatedById",
                schema: "hr",
                table: "JobCandidateFilterSetting",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_JobCandidateFilterSetting_DeletedById",
                schema: "hr",
                table: "JobCandidateFilterSetting",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_JobCandidateFilterSetting_GenderId",
                schema: "hr",
                table: "JobCandidateFilterSetting",
                column: "GenderId");

            migrationBuilder.CreateIndex(
                name: "IX_JobCandidateFilterSetting_JobId",
                schema: "hr",
                table: "JobCandidateFilterSetting",
                column: "JobId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JobCandidateFilterSetting_UpdatedById",
                schema: "hr",
                table: "JobCandidateFilterSetting",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_JobCandidateNationalityPercentage_CandidateTypeId",
                schema: "hr",
                table: "JobCandidateNationalityPercentage",
                column: "CandidateTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_JobCandidateNationalityPercentage_CreatedById",
                schema: "hr",
                table: "JobCandidateNationalityPercentage",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_JobCandidateNationalityPercentage_DeletedById",
                schema: "hr",
                table: "JobCandidateNationalityPercentage",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_JobCandidateNationalityPercentage_JobCandidateFilterSettingId",
                schema: "hr",
                table: "JobCandidateNationalityPercentage",
                column: "JobCandidateFilterSettingId");

            migrationBuilder.CreateIndex(
                name: "IX_JobCandidateNationalityPercentage_NationalityId",
                schema: "hr",
                table: "JobCandidateNationalityPercentage",
                column: "NationalityId");

            migrationBuilder.CreateIndex(
                name: "IX_JobCandidateNationalityPercentage_UpdatedById",
                schema: "hr",
                table: "JobCandidateNationalityPercentage",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_JobCandidateTypePercentage_CandidateTypeId",
                schema: "hr",
                table: "JobCandidateTypePercentage",
                column: "CandidateTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_JobCandidateTypePercentage_CreatedById",
                schema: "hr",
                table: "JobCandidateTypePercentage",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_JobCandidateTypePercentage_DeletedById",
                schema: "hr",
                table: "JobCandidateTypePercentage",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_JobCandidateTypePercentage_JobCandidateFilterSettingId",
                schema: "hr",
                table: "JobCandidateTypePercentage",
                column: "JobCandidateFilterSettingId");

            migrationBuilder.CreateIndex(
                name: "IX_JobCandidateTypePercentage_UpdatedById",
                schema: "hr",
                table: "JobCandidateTypePercentage",
                column: "UpdatedById");

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
                name: "IX_JobCategoryCandidateSettings_CreatedById",
                schema: "hr",
                table: "JobCategoryCandidateSettings",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_JobCategoryCandidateSettings_DeletedById",
                schema: "hr",
                table: "JobCategoryCandidateSettings",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_JobCategoryCandidateSettings_UpdatedById",
                schema: "hr",
                table: "JobCategoryCandidateSettings",
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
                name: "IX_JobPointConfiguration_CreatedById",
                schema: "hr",
                table: "JobPointConfiguration",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_JobPointConfiguration_DeletedById",
                schema: "hr",
                table: "JobPointConfiguration",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_JobPointConfiguration_UpdatedById",
                schema: "hr",
                table: "JobPointConfiguration",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_JobPointsDetail_CreatedById",
                schema: "hr",
                table: "JobPointsDetail",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_JobPointsDetail_DeletedById",
                schema: "hr",
                table: "JobPointsDetail",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_JobPointsDetail_JobPointsMainId",
                schema: "hr",
                table: "JobPointsDetail",
                column: "JobPointsMainId");

            migrationBuilder.CreateIndex(
                name: "IX_JobPointsDetail_UpdatedById",
                schema: "hr",
                table: "JobPointsDetail",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_JobPointsMain_CreatedById",
                schema: "hr",
                table: "JobPointsMain",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_JobPointsMain_DeletedById",
                schema: "hr",
                table: "JobPointsMain",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_JobPointsMain_JobId",
                schema: "hr",
                table: "JobPointsMain",
                column: "JobId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JobPointsMain_UpdatedById",
                schema: "hr",
                table: "JobPointsMain",
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
                name: "IX_JobReviewAttachment_AttachmentId",
                schema: "hr",
                table: "JobReviewAttachment",
                column: "AttachmentId");

            migrationBuilder.CreateIndex(
                name: "IX_JobReviewAttachment_CreatedById",
                schema: "hr",
                table: "JobReviewAttachment",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_JobReviewAttachment_DeletedById",
                schema: "hr",
                table: "JobReviewAttachment",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_JobReviewAttachment_JobId",
                schema: "hr",
                table: "JobReviewAttachment",
                column: "JobId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JobReviewAttachment_JobId_AttachmentId_ReviewCycleId",
                schema: "hr",
                table: "JobReviewAttachment",
                columns: new[] { "JobId", "AttachmentId", "ReviewCycleId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JobReviewAttachment_UpdatedById",
                schema: "hr",
                table: "JobReviewAttachment",
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
                name: "IX_KawaderQids_CreatedById",
                table: "KawaderQids",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_KawaderQids_DeletedById",
                table: "KawaderQids",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_KawaderQids_Qid",
                table: "KawaderQids",
                column: "Qid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_KawaderQids_UpdatedById",
                table: "KawaderQids",
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
                name: "IX_MajorSkill_CreatedById",
                schema: "hr",
                table: "MajorSkill",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_MajorSkill_DeletedById",
                schema: "hr",
                table: "MajorSkill",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_MajorSkill_MajorId",
                schema: "hr",
                table: "MajorSkill",
                column: "MajorId");

            migrationBuilder.CreateIndex(
                name: "IX_MajorSkill_SkillId",
                schema: "hr",
                table: "MajorSkill",
                column: "SkillId");

            migrationBuilder.CreateIndex(
                name: "IX_MajorSkill_UpdatedById",
                schema: "hr",
                table: "MajorSkill",
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
                name: "IX_Office_OfficeAdminId",
                schema: "lkp",
                table: "Office",
                column: "OfficeAdminId");

            migrationBuilder.CreateIndex(
                name: "IX_Office_UpdatedById",
                schema: "lkp",
                table: "Office",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_OfficeSupportedCountry_CountryId",
                schema: "lkp",
                table: "OfficeSupportedCountry",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_OfficeSupportedCountry_CreatedById",
                schema: "lkp",
                table: "OfficeSupportedCountry",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_OfficeSupportedCountry_DeletedById",
                schema: "lkp",
                table: "OfficeSupportedCountry",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_OfficeSupportedCountry_OfficeId",
                schema: "lkp",
                table: "OfficeSupportedCountry",
                column: "OfficeId");

            migrationBuilder.CreateIndex(
                name: "IX_OfficeSupportedCountry_UpdatedById",
                schema: "lkp",
                table: "OfficeSupportedCountry",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Permission_BackendName",
                schema: "lkp",
                table: "Permission",
                column: "BackendName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Permission_CreatedById",
                schema: "lkp",
                table: "Permission",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Permission_DeletedById",
                schema: "lkp",
                table: "Permission",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_Permission_DisplayOrder",
                schema: "lkp",
                table: "Permission",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_Permission_UpdatedById",
                schema: "lkp",
                table: "Permission",
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
                name: "IX_ProfileChangeRequest_CreatedById",
                schema: "hr",
                table: "ProfileChangeRequest",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileChangeRequest_DeletedById",
                schema: "hr",
                table: "ProfileChangeRequest",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileChangeRequest_UpdatedById",
                schema: "hr",
                table: "ProfileChangeRequest",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileChangeRequest_UserProfileId_Section_Status",
                schema: "hr",
                table: "ProfileChangeRequest",
                columns: new[] { "UserProfileId", "Section", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_ProfileChangeRequest_UserProfileId_TargetKey",
                schema: "hr",
                table: "ProfileChangeRequest",
                columns: new[] { "UserProfileId", "TargetKey" },
                unique: true,
                filter: "[Status] IN (1,2)");

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
                name: "IX_Skill_BackendName",
                schema: "lkp",
                table: "Skill",
                column: "BackendName",
                unique: true);

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
                name: "IX_Skill_DisplayOrder",
                schema: "lkp",
                table: "Skill",
                column: "DisplayOrder");

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
                name: "IX_University_LogoArId",
                schema: "lkp",
                table: "University",
                column: "LogoArId");

            migrationBuilder.CreateIndex(
                name: "IX_University_LogoEnId",
                schema: "lkp",
                table: "University",
                column: "LogoEnId");

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
                name: "IX_UserProfileLogger_ActionType",
                schema: "hr",
                table: "UserProfileLogger",
                column: "ActionType");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfileLogger_CreatedById",
                schema: "hr",
                table: "UserProfileLogger",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfileLogger_DeletedById",
                schema: "hr",
                table: "UserProfileLogger",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfileLogger_PerformedById",
                schema: "hr",
                table: "UserProfileLogger",
                column: "PerformedById");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfileLogger_ReviewStatus",
                schema: "hr",
                table: "UserProfileLogger",
                column: "ReviewStatus");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfileLogger_UpdatedById",
                schema: "hr",
                table: "UserProfileLogger",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfileLogger_UserProfileId",
                schema: "hr",
                table: "UserProfileLogger",
                column: "UserProfileId");

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
                name: "FK_AspNetUsers_EmployeeProfile_EmployeeProfileId",
                table: "AspNetUsers",
                column: "EmployeeProfileId",
                principalTable: "EmployeeProfile",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Office_OfficeId",
                table: "AspNetUsers",
                column: "OfficeId",
                principalSchema: "lkp",
                principalTable: "Office",
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
                name: "FK_Country_AspNetUsers_CreatedById",
                schema: "lkp",
                table: "Country");

            migrationBuilder.DropForeignKey(
                name: "FK_Country_AspNetUsers_DeletedById",
                schema: "lkp",
                table: "Country");

            migrationBuilder.DropForeignKey(
                name: "FK_Country_AspNetUsers_UpdatedById",
                schema: "lkp",
                table: "Country");

            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeProfile_AspNetUsers_CreatedById",
                table: "EmployeeProfile");

            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeProfile_AspNetUsers_DeletedById",
                table: "EmployeeProfile");

            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeProfile_AspNetUsers_UpdatedById",
                table: "EmployeeProfile");

            migrationBuilder.DropForeignKey(
                name: "FK_Office_AspNetUsers_CreatedById",
                schema: "lkp",
                table: "Office");

            migrationBuilder.DropForeignKey(
                name: "FK_Office_AspNetUsers_DeletedById",
                schema: "lkp",
                table: "Office");

            migrationBuilder.DropForeignKey(
                name: "FK_Office_AspNetUsers_OfficeAdminId",
                schema: "lkp",
                table: "Office");

            migrationBuilder.DropForeignKey(
                name: "FK_Office_AspNetUsers_UpdatedById",
                schema: "lkp",
                table: "Office");

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
                name: "JobCandidateNationalityPercentage",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "JobCandidateTypePercentage",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "JobCategoryCandidateSettings",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "JobCondition",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "JobDegree",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "JobPointConfiguration",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "JobPointsDetail",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "JobRequiredAttachment",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "JobResponsibility",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "JobReviewAttachment",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "JobSkill",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "JobTabReviewNote",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "KawaderQids");

            migrationBuilder.DropTable(
                name: "LoginAttempt");

            migrationBuilder.DropTable(
                name: "MajorSkill",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "OfficeSupportedCountry",
                schema: "lkp");

            migrationBuilder.DropTable(
                name: "Permission",
                schema: "lkp");

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
                name: "RefreshToken");

            migrationBuilder.DropTable(
                name: "ReviewItem",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "TrainingCourse",
                schema: "pro");

            migrationBuilder.DropTable(
                name: "UserProfileLogger",
                schema: "hr");

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
                name: "Invitation",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "JobCandidateFilterSetting",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "JobPointsMain",
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
                name: "Skill",
                schema: "lkp");

            migrationBuilder.DropTable(
                name: "ProfileChangeRequest",
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
                name: "Job",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "SkillType",
                schema: "lkp");

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
                name: "EmployeeProfile");

            migrationBuilder.DropTable(
                name: "Office",
                schema: "lkp");

            migrationBuilder.DropTable(
                name: "UserType",
                schema: "lkp");

            migrationBuilder.DropTable(
                name: "Country",
                schema: "lkp");
        }
    }
}
