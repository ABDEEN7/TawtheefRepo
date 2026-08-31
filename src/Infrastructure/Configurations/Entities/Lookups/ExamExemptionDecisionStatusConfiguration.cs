using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Infrastructure.Configurations.Entities.Lookups;

public class ExamExemptionDecisionStatusConfiguration : LookupBaseConfiguration<ExamExemptionDecisionStatus>
{
    public override void Configure(EntityTypeBuilder<ExamExemptionDecisionStatus> builder)
    {
        base.Configure(builder);
        builder.HasData(
            new ExamExemptionDecisionStatus
            {
                Id = ExamExemptionDecisionStatusIds.Proposed,
                BackendName = nameof(ExamExemptionDecisionStatusIds.Proposed),
                NameEn = "Proposed",
                NameAr = "مقترح",
                DisplayOrder = 1
            },
            new ExamExemptionDecisionStatus
            {
                Id = ExamExemptionDecisionStatusIds.Approved,
                BackendName = nameof(ExamExemptionDecisionStatusIds.Approved),
                NameEn = "Approved",
                NameAr = "معتمد",
                DisplayOrder = 2
            },
            new ExamExemptionDecisionStatus
            {
                Id = ExamExemptionDecisionStatusIds.Rejected,
                BackendName = nameof(ExamExemptionDecisionStatusIds.Rejected),
                NameEn = "Rejected",
                NameAr = "مرفوض",
                DisplayOrder = 3
            },
            new ExamExemptionDecisionStatus
            {
                Id = ExamExemptionDecisionStatusIds.Cancelled,
                BackendName = nameof(ExamExemptionDecisionStatusIds.Cancelled),
                NameEn = "Cancelled",
                NameAr = "ملغي",
                DisplayOrder = 4
            }
        );
    }
}

