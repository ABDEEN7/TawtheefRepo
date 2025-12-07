using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Infrastructure.Configurations.Entities.Lookups;

public class JobStatusConfiguration : LookupBaseConfiguration<JobStatus>
{
    public override void Configure(EntityTypeBuilder<JobStatus> builder)
    {
        base.Configure(builder);

        builder.HasData(
            new JobStatus
            {
                Id = JobStatusIds.Draft,
                BackendName = nameof(JobStatusIds.Draft),
                NameEn = "Draft",
                NameAr = "مسودة",
                DescriptionEn = "Job is created as a draft and not visible to the public.",
                DescriptionAr = "تم إنشاء الوظيفة كمسودة ولم يتم إرسالها للجمهور.",
                DisplayOrder = 1
            },
            new JobStatus
            {
                Id = JobStatusIds.Active,
                BackendName = nameof(JobStatusIds.Active),
                NameEn = "Active",
                NameAr = "نشطة",
                DescriptionEn = "Job is open for applications and can be published to the target audience.",
                DescriptionAr = "الوظيفة جاهزة للتقديم ويمكن إرسالها للجمهور المطلوب.",
                DisplayOrder = 2
            },
            new JobStatus
            {
                Id = JobStatusIds.Closed,
                BackendName = nameof(JobStatusIds.Closed),
                NameEn = "Closed",
                NameAr = "متوقفة",
                DescriptionEn = "Application end date has been reached; job is not accepting new applications.",
                DescriptionAr = "تم الوصول إلى تاريخ نهاية التقديم ولن يُسمح بإرسال الطلبات.",
                DisplayOrder = 3
            },
            new JobStatus
            {
                Id = JobStatusIds.Cancelled,
                BackendName = nameof(JobStatusIds.Cancelled),
                NameEn = "Cancelled",
                NameAr = "ملغية",
                DescriptionEn = "The job has been cancelled and will not be published on the platform.",
                DescriptionAr = "تم إلغاء الوظيفة ككل ولن يتم نشرها على المنصة.",
                DisplayOrder = 4
            },

            new JobStatus
            {
                Id = JobStatusIds.PendingApproval,
                BackendName = nameof(JobStatusIds.PendingApproval),
                NameEn = "Pending Approval",
                NameAr = "قيد الموافقة",
                DescriptionEn = "Job is waiting for approval.",
                DescriptionAr = "الوظيفة قيد الموافقة.",
                DisplayOrder = 5
            },
            new JobStatus
            {
                Id = JobStatusIds.Approved,
                BackendName = nameof(JobStatusIds.Approved),
                NameEn = "Approved",
                NameAr = "معتمدة",
                DescriptionEn = "Job was approved.",
                DescriptionAr = "تم اعتماد الوظيفة.",
                DisplayOrder = 6
            },
            new JobStatus
            {
                Id = JobStatusIds.ReadyForAnnouncement,
                BackendName = nameof(JobStatusIds.ReadyForAnnouncement),
                NameEn = "Ready For Announcement",
                NameAr = "جاهزة للإعلان",
                DescriptionEn = "Job is ready to be announced.",
                DescriptionAr = "الوظيفة جاهزة للإعلان.",
                DisplayOrder = 7
            },
            new JobStatus
            {
                Id = JobStatusIds.Published,
                BackendName = nameof(JobStatusIds.Published),
                NameEn = "Published",
                NameAr = "منشورة",
                DescriptionEn = "Job is published.",
                DescriptionAr = "تم نشر الوظيفة.",
                DisplayOrder = 8
            },
            new JobStatus
            {
                Id = JobStatusIds.Rejected,
                BackendName = nameof(JobStatusIds.Rejected),
                NameEn = "Rejected",
                NameAr = "مرفوضة",
                DescriptionEn = "Job was rejected.",
                DescriptionAr = "تم رفض الوظيفة.",
                DisplayOrder = 9
            }
        );
    }
}
