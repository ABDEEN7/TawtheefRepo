using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Infrastructure.Configurations.Entities.Lookups;

public class StageConfiguration : LookupBaseConfiguration<Stage>
{
    public override void Configure(EntityTypeBuilder<Stage> builder)
    {
        base.Configure(builder);

        builder.HasData(
            new Stage
            {
                Id = StageIds.Primary,
                BackendName = "PRIMARY",
                NameEn = "Primary",
                NameAr = "المرحلة الابتدائية",
                DisplayOrder = 1
            },
            new Stage
            {
                Id = StageIds.Preparatory,
                BackendName = "PREPARATORY",
                NameEn = "Preparatory",
                NameAr = "المرحلة الإعدادية",
                DisplayOrder = 2
            },
            new Stage
            {
                Id = StageIds.Secondary,
                BackendName = "SECONDARY",
                NameEn = "Secondary",
                NameAr = "المرحلة الثانوية",
                DisplayOrder = 3
            }
        );
    }
}
