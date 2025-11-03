using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Domain.Configurations.Entities.Lookups;

public class MaritalStatusConfiguration : LookupBaseConfiguration<MaritalStatus>
{
    public override void Configure(EntityTypeBuilder<MaritalStatus> builder)
    {
        builder.HasData(
            new MaritalStatus
            {
                Id = MaritalStatusIds.Single,
                BackendName = nameof(MaritalStatusIds.Single),
                NameEn = "Single",
                NameAr = "أعزب",
                DisplayOrder = 1
            },
            new MaritalStatus
            {
                Id = MaritalStatusIds.Married,
                BackendName = nameof(MaritalStatusIds.Married),
                NameEn = "Married",
                NameAr = "متزوج",
                DisplayOrder = 2
            },
            new MaritalStatus
            {
                Id = MaritalStatusIds.Divorced,
                BackendName = nameof(MaritalStatusIds.Divorced),
                NameEn = "Divorced",
                NameAr = "مطلق",
                DisplayOrder = 3
            },
            new MaritalStatus
            {
                Id = MaritalStatusIds.Widowed,
                BackendName = nameof(MaritalStatusIds.Widowed),
                NameEn = "Widowed",
                NameAr = "أرمل",
                DisplayOrder = 4
            }
        );
        base.Configure(builder);
    }
}
