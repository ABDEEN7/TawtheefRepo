using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Infrastructure.Configurations.Entities.Lookups;

public class ExamResultReportStatusConfiguration : LookupBaseConfiguration<ExamResultReportStatus>
{
    public override void Configure(EntityTypeBuilder<ExamResultReportStatus> builder)
    {
        base.Configure(builder);
        builder.HasData(
            new ExamResultReportStatus
            {
                Id = ExamResultReportStatusIds.Creating,
                BackendName = nameof(ExamResultReportStatusIds.Creating),
                NameEn = "Creating",
                NameAr = "قيد الإنشاء",
                DisplayOrder = 1
            },
            new ExamResultReportStatus
            {
                Id = ExamResultReportStatusIds.UnderReview,
                BackendName = nameof(ExamResultReportStatusIds.UnderReview),
                NameEn = "Under Review",
                NameAr = "قيد المراجعة",
                DisplayOrder = 2
            },
            new ExamResultReportStatus
            {
                Id = ExamResultReportStatusIds.Returned,
                BackendName = nameof(ExamResultReportStatusIds.Returned),
                NameEn = "Returned",
                NameAr = "معاد",
                DisplayOrder = 3
            },
            new ExamResultReportStatus
            {
                Id = ExamResultReportStatusIds.Approved,
                BackendName = nameof(ExamResultReportStatusIds.Approved),
                NameEn = "Approved",
                NameAr = "معتمد",
                DisplayOrder = 4
            },
            new ExamResultReportStatus
            {
                Id = ExamResultReportStatusIds.Closed,
                BackendName = nameof(ExamResultReportStatusIds.Closed),
                NameEn = "Closed",
                NameAr = "مغلق",
                DisplayOrder = 5
            }
        );
    }
}

