using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Domain.Configurations.Entities.Lookups;

public class TargetEntityConfiguration : LookupBaseConfiguration<TargetEntity>
{
    public override void Configure(EntityTypeBuilder<TargetEntity> builder)
    {
        builder.HasData(
            new TargetEntity
            {
                Id = TargetEntityIds.Schools,
                BackendName = nameof(TargetEntityIds.Schools),
                NameEn = "Schools",
                NameAr = "المدارس",
                DisplayOrder = 1
            },
            new TargetEntity
            {
                Id = TargetEntityIds.Ministry,
                BackendName = nameof(TargetEntityIds.Ministry),
                NameEn = "Ministry",
                NameAr = "الوزارة",
                DisplayOrder = 2
            }
        );
        base.Configure(builder);
    }
}
