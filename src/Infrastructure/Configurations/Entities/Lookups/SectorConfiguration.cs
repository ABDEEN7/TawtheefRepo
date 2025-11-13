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
    }
}
