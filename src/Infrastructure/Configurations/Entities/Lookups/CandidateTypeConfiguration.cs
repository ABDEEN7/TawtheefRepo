using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Infrastructure.Configurations.Entities.Lookups;

public class CandidateTypeConfiguration : LookupBaseConfiguration<CandidateType>
{
    public override void Configure(EntityTypeBuilder<CandidateType> builder)
    {
        base.Configure(builder);
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
                NameEn = "Resident outside Qatar",
                NameAr = "مقيم خارج قطر",
                DisplayOrder = 4
            },
            new CandidateType
            {
                Id = CandidateTypeIds.SonOfQatariMother,
                BackendName = nameof(CandidateTypeIds.SonOfQatariMother),
                NameAr = "أبناء المرأة القطرية المتزوجة من غير قطري",
                NameEn = "Children of a Qatari woman married to a non-Qatari",
                DisplayOrder = 5
            },
            new CandidateType
            {
                Id = CandidateTypeIds.WifeOfQatari,
                BackendName = nameof(CandidateTypeIds.WifeOfQatari),
                NameEn = "Non-Qatari husband married to a Qatari woman or man",
                NameAr = "الزوج غير القطري المتزوج من قطرية أو قطري",
                DisplayOrder = 6
            }
        );
    }
}
