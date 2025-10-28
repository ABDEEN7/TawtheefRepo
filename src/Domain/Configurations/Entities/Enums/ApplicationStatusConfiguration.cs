using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Domain.Configurations.Entities.Enums;

public class ApplicationStatusConfiguration : LookupBaseConfiguration<ApplicationStatus>
{
    public override void Configure(EntityTypeBuilder<ApplicationStatus> builder)
    {
        builder.HasData(
            new ApplicationStatus
            {
                Id = ApplicationStatusIds.Submitted,
                BackendName = nameof(ApplicationStatusIds.Submitted),
                NameEn = "Submitted",
                NameAr = "تم الإرسال",
                DescriptionEn = "The application has been submitted by the candidate.",
                DescriptionAr = "تم إرسال الطلب من قبل المتقدم.",
                DisplayOrder = 1
            },
            new ApplicationStatus
            {
                Id = ApplicationStatusIds.Returned,
                BackendName = nameof(ApplicationStatusIds.Returned),
                NameEn = "Returned",
                NameAr = "تم الإرجاع",
                DescriptionEn = "Returned to the applicant by the recruiter due to missing or incorrect information.",
                DescriptionAr = "تم إرجاع الطلب من قبل مسؤول التوظيف لوجود نقص أو خطأ.",
                DisplayOrder = 2
            },
            new ApplicationStatus
            {
                Id = ApplicationStatusIds.UnderReview,
                BackendName = nameof(ApplicationStatusIds.UnderReview),
                NameEn = "Under Review",
                NameAr = "تحت الفحص",
                DescriptionEn = "Administrative/technical review of attachments and profile is in progress.",
                DescriptionAr = "تتم عملية فحص المرفقات والملف الشخصي.",
                DisplayOrder = 3
            },
            new ApplicationStatus
            {
                Id = ApplicationStatusIds.AdminShortlisting,
                BackendName = nameof(ApplicationStatusIds.AdminShortlisting),
                NameEn = "Administrative Shortlisting",
                NameAr = "فرز إداري",
                DescriptionEn = "Administrative shortlisting by recruiter is in progress (recruiter-only visibility).",
                DescriptionAr = "يتم حالياً الفرز الإداري من قبل مسؤول التوظيف (تظهر لمسؤول التوظيف فقط).",
                DisplayOrder = 4
            },
            new ApplicationStatus
            {
                Id = ApplicationStatusIds.TechnicalShortlisting,
                BackendName = nameof(ApplicationStatusIds.TechnicalShortlisting),
                NameEn = "Technical Shortlisting",
                NameAr = "فرز فني",
                DescriptionEn = "Technical shortlisting is in progress (recruiter-only visibility).",
                DescriptionAr = "يتم حالياً الفرز الفني من قبل الموجه (تظهر لمسؤول التوظيف فقط).",
                DisplayOrder = 5
            },
            new ApplicationStatus
            {
                Id = ApplicationStatusIds.TestProcessing,
                BackendName = nameof(ApplicationStatusIds.TestProcessing),
                NameEn = "Test Processing",
                NameAr = "تحت إجراءات الاختبار",
                DescriptionEn = "Testing procedures are being arranged/executed (results visible to recruiter).",
                DescriptionAr = "يتم حالياً تنفيذ الإجراءات الخاصة بالاختبار (تظهر النتائج لمسؤول التوظيف).",
                DisplayOrder = 6
            },
            new ApplicationStatus
            {
                Id = ApplicationStatusIds.InterviewProcessing,
                BackendName = nameof(ApplicationStatusIds.InterviewProcessing),
                NameEn = "Interview Processing",
                NameAr = "تحت إجراءات المقابلات",
                DescriptionEn = "Interview procedures are being arranged/executed (results visible to recruiter).",
                DescriptionAr = "يتم حالياً تنفيذ الإجراءات الخاصة بالمقابلات (تظهر النتائج لمسؤول التوظيف).",
                DisplayOrder = 7,
            },
            new ApplicationStatus
            {
                Id = ApplicationStatusIds.HiringProcessing,
                BackendName = nameof(ApplicationStatusIds.HiringProcessing),
                NameEn = "Hiring Processing",
                NameAr = "تحت إجراءات التعيين",
                DescriptionEn = "Hiring actions are in progress (recruiter-only results).",
                DescriptionAr = "يتم حالياً تنفيذ الإجراءات الخاصة بالتعيين (تظهر النتائج لمسؤول التوظيف).",
                DisplayOrder = 8
            },
            new ApplicationStatus
            {
                Id = ApplicationStatusIds.FinalApprovalProcessing,
                BackendName = nameof(ApplicationStatusIds.FinalApprovalProcessing),
                NameEn = "Final Approval Processing",
                NameAr = "تحت الإعتماد النهائي",
                DescriptionEn = "Final approval actions are in progress (recruiter-only results).",
                DescriptionAr = "يتم حالياً تنفيذ الإجراءات الخاصة بالاعتماد النهائي (تظهر النتائج لمسؤول التوظيف).",
                DisplayOrder = 9
            }
        );

        base.Configure(builder);
    }
}
