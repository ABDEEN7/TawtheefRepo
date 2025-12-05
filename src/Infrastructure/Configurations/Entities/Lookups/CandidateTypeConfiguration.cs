using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Common;
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
                DescriptionAr = "قطري الجنسية",
                DescriptionEn = "Qatari National",
                DisplayOrder = 1
            },
            new CandidateType
            {
                Id = CandidateTypeIds.GCC,
                BackendName = nameof(CandidateTypeIds.GCC),
                NameEn = "Citizen of a GCC country",
                NameAr = "مواطن دول مجلس التعاون الخليجي",
                DescriptionAr = "مواطن دول مجلس التعاون الخليجي",
                DescriptionEn = "Citizen of a GCC country",
                DisplayOrder = 2
            },
            new CandidateType
            {
                Id = CandidateTypeIds.ResidentQatar,
                BackendName = nameof(CandidateTypeIds.ResidentQatar),
                NameEn = "Resident in Qatar",
                NameAr = "مقيم في قطر",
                DescriptionAr = "مقيم داخل دولة قطر",
                DescriptionEn = "Resident in Qatar",
                DisplayOrder = 3
            },
            new CandidateType
            {
                Id = CandidateTypeIds.NonQatari,
                BackendName = nameof(CandidateTypeIds.NonQatari),
                NameEn = "Resident outside Qatar",
                NameAr = "مقيم خارج قطر",
                DescriptionAr = "مقيم خارج دولة قطر",
                DescriptionEn = "Resident outside Qatar",
                DisplayOrder = 4
            },
            new CandidateType
            {
                Id = CandidateTypeIds.SonOfQatariMother,
                BackendName = nameof(CandidateTypeIds.SonOfQatariMother),
                NameAr = "أبناء المرأة القطرية المتزوجة من غير قطري",
                NameEn = "Children of a Qatari woman married to a non-Qatari",
                DescriptionAr = "أبناء المرأة القطرية المتزوجة من غير قطري",
                DescriptionEn = "Children of a Qatari woman married to a non-Qatari",
                DisplayOrder = 5
            },
            new CandidateType
            {
                Id = CandidateTypeIds.WifeOfQatari,
                BackendName = nameof(CandidateTypeIds.WifeOfQatari),
                NameEn = "Non-Qatari spouse married to a Qatari",
                NameAr = "الزوج أو الزوجة غير قطري/ة المتزوج/ة من قطري/ة",
                DescriptionAr = "الزوج أو الزوجة غير قطري/ة المتزوج/ة من قطري/ة",
                DescriptionEn = "Non-Qatari spouse married to a Qatari",
                DisplayOrder = 6
            }
        );
    }
}
