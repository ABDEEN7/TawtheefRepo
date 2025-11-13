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
                DescriptionAr = "تم إلغاء الوظيفة ككل ولن يتم نشرها على المنصة (تظهر لمسؤول التوظيف فقط).",
                DisplayOrder = 4
            }
        );
    }
}
