using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Domain.Configurations.Entities.Lookups;

public class CandidateTypeConfiguration : LookupBaseConfiguration<CandidateType>
{
    public override void Configure(EntityTypeBuilder<CandidateType> builder)
    {
        builder.HasData(
            new CandidateType
            {
                Id = CandidateTypeIds.Qatari,
                BackendName = nameof(CandidateTypeIds.Qatari),
                NameEn = "Qatari",
                NameAr = "قطري",
                DisplayOrder = 1
            },
            new CandidateType
            {
                Id = CandidateTypeIds.GCC,
                BackendName = nameof(CandidateTypeIds.GCC),
                NameEn = "GCC National",
                NameAr = "مجلس تعاون الخليج",
                DisplayOrder = 2
            },
            new CandidateType
            {
                Id = CandidateTypeIds.ResidentQatar,
                BackendName = nameof(CandidateTypeIds.ResidentQatar),
                NameEn = "Resident in Qatar",
                NameAr = "مقيم في قطر",
                DisplayOrder = 3
            },
            new CandidateType
            {
                Id = CandidateTypeIds.NonQatari,
                BackendName = nameof(CandidateTypeIds.NonQatari),
                NameEn = "Non-Qatari",
                NameAr = "غير قطري",
                DisplayOrder = 4
            }
        );
        base.Configure(builder);
    }
}
