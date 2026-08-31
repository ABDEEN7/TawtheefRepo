using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Infrastructure.Configurations.Entities.Lookups;

public class ExamStatusConfiguration : LookupBaseConfiguration<ExamStatus>
{
    public override void Configure(EntityTypeBuilder<ExamStatus> builder)
    {
        base.Configure(builder);
        builder.HasData(
            new ExamStatus
            {
                Id = ExamStatusIds.Draft,
                BackendName = nameof(ExamStatusIds.Draft),
                NameEn = "Draft",
                NameAr = "مسودة",
                DisplayOrder = 1
            },
            new ExamStatus
            {
                Id = ExamStatusIds.PendingApproval,
                BackendName = nameof(ExamStatusIds.PendingApproval),
                NameEn = "Pending Approval",
                NameAr = "قيد الاعتماد",
                DisplayOrder = 2
            },
            new ExamStatus
            {
                Id = ExamStatusIds.Returned,
                BackendName = nameof(ExamStatusIds.Returned),
                NameEn = "Returned",
                NameAr = "معاد",
                DisplayOrder = 3
            },
            new ExamStatus
            {
                Id = ExamStatusIds.Approved,
                BackendName = nameof(ExamStatusIds.Approved),
                NameEn = "Approved",
                NameAr = "معتمد",
                DisplayOrder = 4
            },
            new ExamStatus
            {
                Id = ExamStatusIds.Cancelled,
                BackendName = nameof(ExamStatusIds.Cancelled),
                NameEn = "Cancelled",
                NameAr = "ملغي",
                DisplayOrder = 5
            }
        );
    }
}

