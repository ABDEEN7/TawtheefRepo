using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Domain.Configurations.Entities.Lookups;

public class SectorConfiguration : LookupBaseConfiguration<Sector>
{
    public override void Configure(EntityTypeBuilder<Sector> builder)
    {
        builder.HasData(
            new Sector
            {
                Id = SectorIds.Schools,
                BackendName = nameof(SectorIds.Schools),
                NameEn = "Schools",
                NameAr = "المدارس",
                DescriptionAr = "قطاع المدارس",
                DescriptionEn = "Sector for schools",
                DisplayOrder = 1
            },
            new Sector
            {
                Id = SectorIds.Ministry,
                BackendName = nameof(SectorIds.Ministry),
                NameEn = "Ministry",
                NameAr = "الوزارة",
                DescriptionAr = "قطاع الوزارة",
                DescriptionEn = "Sector for ministry",
                DisplayOrder = 2
            }
        );
        base.Configure(builder);
    }
}
