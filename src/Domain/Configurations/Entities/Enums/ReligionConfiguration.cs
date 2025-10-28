using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Domain.Configurations.Entities.Enums;

public class ReligionConfiguration : LookupBaseConfiguration<Religion>
{
    public override void Configure(EntityTypeBuilder<Religion> builder)
    {
        builder.HasData(
            new Religion
            {
                Id = ReligionIds.Islam,
                BackendName = nameof(ReligionIds.Islam),
                NameEn = "Islam",
                NameAr = "الإسلام",
                DisplayOrder = 1
            },
            new Religion
            {
                Id = ReligionIds.Christian,
                BackendName = nameof(ReligionIds.Christian),
                NameEn = "Christianity",
                NameAr = "المسيحية",
                DisplayOrder = 2
            },
            new Religion
            {
                Id = ReligionIds.Hindu,
                BackendName = nameof(ReligionIds.Hindu),
                NameEn = "Hinduism",
                NameAr = "الهندوسية",
                DisplayOrder = 3
            },
            new Religion
            {
                Id = ReligionIds.Buddhist,
                BackendName = nameof(ReligionIds.Buddhist),
                NameEn = "Buddhism",
                NameAr = "البوذية",
                DisplayOrder = 4
            },
            new Religion
            {
                Id = ReligionIds.Sikh,
                BackendName = nameof(ReligionIds.Sikh),
                NameEn = "Sikhism",
                NameAr = "السيخية",
                DisplayOrder = 5
            },
            new Religion
            {
                Id = ReligionIds.Other,
                BackendName = nameof(ReligionIds.Other),
                NameEn = "Other",
                NameAr = "أخرى",
                DisplayOrder = 6
            }
        );
        base.Configure(builder);
    }
}
