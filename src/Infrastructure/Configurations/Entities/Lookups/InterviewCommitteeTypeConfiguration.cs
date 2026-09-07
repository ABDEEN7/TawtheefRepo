using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Infrastructure.Configurations.Entities.Lookups;

public class InterviewCommitteeTypeConfiguration : LookupBaseConfiguration<InterviewCommitteeType>
{
    public override void Configure(EntityTypeBuilder<InterviewCommitteeType> builder)
    {
        base.Configure(builder);
        builder.HasData(
            new InterviewCommitteeType
            {
                Id = InterviewCommitteeTypeIds.Academic,
                BackendName = nameof(InterviewCommitteeTypeIds.Academic),
                NameEn = "Academic",
                NameAr = "أكاديمية",
                DisplayOrder = 1
            },
            new InterviewCommitteeType
            {
                Id = InterviewCommitteeTypeIds.Administrative,
                BackendName = nameof(InterviewCommitteeTypeIds.Administrative),
                NameEn = "Administrative",
                NameAr = "إدارية",
                DisplayOrder = 2
            },
            new InterviewCommitteeType
            {
                Id = InterviewCommitteeTypeIds.Labor,
                BackendName = nameof(InterviewCommitteeTypeIds.Labor),
                NameEn = "Labor",
                NameAr = "عمالية",
                DisplayOrder = 3
            },
            new InterviewCommitteeType
            {
                Id = InterviewCommitteeTypeIds.Other,
                BackendName = nameof(InterviewCommitteeTypeIds.Other),
                NameEn = "Other",
                NameAr = "أخرى",
                DisplayOrder = 4
            }
        );
    }
}
