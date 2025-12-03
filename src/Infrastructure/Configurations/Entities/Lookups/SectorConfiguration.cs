using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Infrastructure.Configurations.Entities.Lookups;

public class SectorConfiguration : LookupBaseConfiguration<Sector>
{
    public override void Configure(EntityTypeBuilder<Sector> builder)
    {
        base.Configure(builder);
        builder.HasData(
            new Sector
            {
                Id = SectorIds.DeputyMinisterSector,
                BackendName = nameof(SectorIds.DeputyMinisterSector),
                NameEn = "Deputy Minister Sector",
                NameAr = "سعادة الوكيل",
                DescriptionAr = "قطاع سعادة الوكيل",
                DescriptionEn = "Deputy Minister Sector",
                DisplayOrder = 3
            },
            new Sector
            {
                Id = SectorIds.GeneralEducationSector,
                BackendName = nameof(SectorIds.GeneralEducationSector),
                NameEn = "General Education Sector",
                NameAr = "التعليم العام",
                DescriptionAr = "قطاع التعليم العام",
                DescriptionEn = "General Education Sector",
                DisplayOrder = 4
            },
            new Sector
            {
                Id = SectorIds.PrivateEducationSector,
                BackendName = nameof(SectorIds.PrivateEducationSector),
                NameEn = "Private Education Sector",
                NameAr = "التعليم الخاص",
                DescriptionAr = "قطاع التعليم الخاص",
                DescriptionEn = "Private Education Sector",
                DisplayOrder = 5
            },
            new Sector
            {
                Id = SectorIds.AssessmentSector,
                BackendName = nameof(SectorIds.AssessmentSector),
                NameEn = "Assessment Sector",
                NameAr = "التقييم",
                DescriptionAr = "قطاع التقييم",
                DescriptionEn = "Assessment Sector",
                DisplayOrder = 6
            },
            new Sector
            {
                Id = SectorIds.SharedServicesSector,
                BackendName = nameof(SectorIds.SharedServicesSector),
                NameEn = "Shared Services Sector",
                NameAr = "الخدمات المشتركة",
                DescriptionAr = "قطاع الخدمات المشتركة",
                DescriptionEn = "Shared Services Sector",
                DisplayOrder = 7
            }
        );
    }
}
