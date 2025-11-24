using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Tawtheef.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDraftModeCreateProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "InvitationStatus",
                keyColumn: "Id",
                keyValue: new Guid("1c2d3e4f-5a6b-7c8d-9e1f-2a3b4c5d6e20"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "InvitationStatus",
                keyColumn: "Id",
                keyValue: new Guid("2e4b6a8c-1d2f-4b6a-9c3e-7a1b2c3d4e55"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "InvitationStatus",
                keyColumn: "Id",
                keyValue: new Guid("3f0b7dab-7c9f-4b3f-8a5f-1f8d2b7f2c11"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "InvitationStatus",
                keyColumn: "Id",
                keyValue: new Guid("5a7d2c4f-3b1e-44a6-a9e5-9e7d23b4c1a2"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "InvitationStatus",
                keyColumn: "Id",
                keyValue: new Guid("6a5b4c3d-2e1f-4a6b-9c8d-1e2f3a4b5c60"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "InvitationStatus",
                keyColumn: "Id",
                keyValue: new Guid("7b9e1c3d-5a2f-4f6b-8a1d-2c3e4f5a6b70"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "InvitationStatus",
                keyColumn: "Id",
                keyValue: new Guid("9c2a7d5b-6e4f-4d1a-83b2-5e7f9a1c2d30"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "InvitationStatus",
                keyColumn: "Id",
                keyValue: new Guid("a1b923d2-2c7e-4e1a-9b14-0b6d2f5c7e90"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "InvitationStatus",
                keyColumn: "Id",
                keyValue: new Guid("d3e2a4b6-8f1c-4e7a-9d2b-7c5f1a9e3b40"));

            migrationBuilder.AddColumn<bool>(
                name: "IsDraft",
                schema: "app",
                table: "UserProfile",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "InvitationStatus",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("22ef7e86-28cb-4a30-98bc-7d45f9b44de3"), "APPROVED", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "تمت الموافقة على الطلب وانتقل لمرحلة لاحقة.", "The application has been approved and moved to a later stage.", 6, false, "معتمد", "Approved", null, null },
                    { new Guid("43ad4950-46a9-4b37-b4e2-4a2d8ac4a7c4"), "UNDER_REVIEW", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "الطلب تحت المراجعة المبدئية لقسم التوظيف.", "The application is under initial review by the recruitment department.", 4, false, "قيد المراجعة", "Under Review", null, null },
                    { new Guid("5602dbce-00a3-488b-9156-981a0fb18a01"), "REQUIRES_UPDATE", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "الطلب تم إرجاعه للمرشح لإكمال نواقص محددة.", "The application was returned to the candidate to complete missing information.", 5, false, "مطلوب تعديل", "Requires Update", null, null },
                    { new Guid("64236c6a-167a-4213-b1d6-80c2c8c86dde"), "READED", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "المرشح فتح الدعوة لأول مرة ولم يبدأ التقديم.", "The candidate opened the invitation for the first time but has not started the application.", 2, false, "تمت القراءة", "Read", null, null },
                    { new Guid("7103ba49-ad43-4751-b1a5-9084aca69676"), "REJECTED", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "الطلب لم يتم قبوله لأسباب وظيفية أو تنظيمية.", "The application was not accepted for functional or organizational reasons.", 7, false, "مرفوض", "Rejected", null, null },
                    { new Guid("bced01d8-3784-4e2c-9312-930951cc59d8"), "CANCELLED", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "المرشح قام بإلغاء الطلب أو تم إلغاؤه وفق الإجراءات.", "The candidate cancelled the application or it was cancelled procedurally.", 8, false, "ملغي", "Cancelled", null, null },
                    { new Guid("ca054592-8617-406d-8e8f-3a773b3d0d5e"), "CLOSED", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "الوظيفة انتهت أو أُغلقت من قبل الموارد البشرية ولا يمكن اتخاذ أي إجراء عليها.", "The job has ended or was closed by HR and no further action can be taken.", 9, false, "مغلق", "Closed", null, null },
                    { new Guid("e7b28f10-fb23-466e-a9b2-aaa3c9afdeac"), "SUBMITTED", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "المرشح قدم طلبه وجميع بياناته مكتملة.", "The candidate submitted the application with all required information completed.", 3, false, "تم التقديم", "Submitted", null, null },
                    { new Guid("f0bc801d-f54c-4a0e-8aae-00694e4fc80d"), "NEW_INVITATION", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "وظيفة تمت دعوة المرشح لها ولم يقم بقراءتها أو فتحها بعد.", "The candidate was invited but has not opened or viewed it yet.", 1, false, "دعوة جديدة", "New Invitation", null, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "InvitationStatus",
                keyColumn: "Id",
                keyValue: new Guid("22ef7e86-28cb-4a30-98bc-7d45f9b44de3"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "InvitationStatus",
                keyColumn: "Id",
                keyValue: new Guid("43ad4950-46a9-4b37-b4e2-4a2d8ac4a7c4"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "InvitationStatus",
                keyColumn: "Id",
                keyValue: new Guid("5602dbce-00a3-488b-9156-981a0fb18a01"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "InvitationStatus",
                keyColumn: "Id",
                keyValue: new Guid("64236c6a-167a-4213-b1d6-80c2c8c86dde"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "InvitationStatus",
                keyColumn: "Id",
                keyValue: new Guid("7103ba49-ad43-4751-b1a5-9084aca69676"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "InvitationStatus",
                keyColumn: "Id",
                keyValue: new Guid("bced01d8-3784-4e2c-9312-930951cc59d8"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "InvitationStatus",
                keyColumn: "Id",
                keyValue: new Guid("ca054592-8617-406d-8e8f-3a773b3d0d5e"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "InvitationStatus",
                keyColumn: "Id",
                keyValue: new Guid("e7b28f10-fb23-466e-a9b2-aaa3c9afdeac"));

            migrationBuilder.DeleteData(
                schema: "lkp",
                table: "InvitationStatus",
                keyColumn: "Id",
                keyValue: new Guid("f0bc801d-f54c-4a0e-8aae-00694e4fc80d"));

            migrationBuilder.DropColumn(
                name: "IsDraft",
                schema: "app",
                table: "UserProfile");

            migrationBuilder.InsertData(
                schema: "lkp",
                table: "InvitationStatus",
                columns: new[] { "Id", "BackendName", "CreatedById", "CreatedDate", "DeletedById", "DeletedDate", "DescriptionAr", "DescriptionEn", "DisplayOrder", "IsDeleted", "NameAr", "NameEn", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("1c2d3e4f-5a6b-7c8d-9e1f-2a3b4c5d6e20"), "FinalApprovalProcessing", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "يتم حالياً تنفيذ الإجراءات الخاصة بالاعتماد النهائي (تظهر النتائج لمسؤول التوظيف).", "Final approval actions are in progress (recruiter-only results).", 9, false, "تحت الإعتماد النهائي", "Final Approval Processing", null, null },
                    { new Guid("2e4b6a8c-1d2f-4b6a-9c3e-7a1b2c3d4e55"), "TestProcessing", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "يتم حالياً تنفيذ الإجراءات الخاصة بالاختبار (تظهر النتائج لمسؤول التوظيف).", "Testing procedures are being arranged/executed (results visible to recruiter).", 6, false, "تحت إجراءات الاختبار", "Test Processing", null, null },
                    { new Guid("3f0b7dab-7c9f-4b3f-8a5f-1f8d2b7f2c11"), "Submitted", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "تم إرسال الطلب من قبل المتقدم.", "The application has been submitted by the candidate.", 1, false, "تم الإرسال", "Submitted", null, null },
                    { new Guid("5a7d2c4f-3b1e-44a6-a9e5-9e7d23b4c1a2"), "UnderReview", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "تتم عملية فحص المرفقات والملف الشخصي.", "Administrative/technical review of attachments and profile is in progress.", 3, false, "تحت الفحص", "Under Review", null, null },
                    { new Guid("6a5b4c3d-2e1f-4a6b-9c8d-1e2f3a4b5c60"), "HiringProcessing", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "يتم حالياً تنفيذ الإجراءات الخاصة بالتعيين (تظهر النتائج لمسؤول التوظيف).", "Hiring actions are in progress (recruiter-only results).", 8, false, "تحت إجراءات التعيين", "Hiring Processing", null, null },
                    { new Guid("7b9e1c3d-5a2f-4f6b-8a1d-2c3e4f5a6b70"), "InterviewProcessing", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "يتم حالياً تنفيذ الإجراءات الخاصة بالمقابلات (تظهر النتائج لمسؤول التوظيف).", "Interview procedures are being arranged/executed (results visible to recruiter).", 7, false, "تحت إجراءات المقابلات", "Interview Processing", null, null },
                    { new Guid("9c2a7d5b-6e4f-4d1a-83b2-5e7f9a1c2d30"), "TechnicalShortlisting", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "يتم حالياً الفرز الفني من قبل الموجه (تظهر لمسؤول التوظيف فقط).", "Technical shortlisting is in progress (recruiter-only visibility).", 5, false, "فرز فني", "Technical Shortlisting", null, null },
                    { new Guid("a1b923d2-2c7e-4e1a-9b14-0b6d2f5c7e90"), "Returned", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "تم إرجاع الطلب من قبل مسؤول التوظيف لوجود نقص أو خطأ.", "Returned to the applicant by the recruiter due to missing or incorrect information.", 2, false, "تم الإرجاع", "Returned", null, null },
                    { new Guid("d3e2a4b6-8f1c-4e7a-9d2b-7c5f1a9e3b40"), "AdminShortlisting", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, "يتم حالياً الفرز الإداري من قبل مسؤول التوظيف (تظهر لمسؤول التوظيف فقط).", "Administrative shortlisting by recruiter is in progress (recruiter-only visibility).", 4, false, "فرز إداري", "Administrative Shortlisting", null, null }
                });
        }
    }
}
