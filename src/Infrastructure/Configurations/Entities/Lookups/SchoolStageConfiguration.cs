using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Infrastructure.Configurations.Entities.Lookups;

public class SchoolStageConfiguration : LookupBaseConfiguration<SchoolStage>
{
    public override void Configure(EntityTypeBuilder<SchoolStage> builder)
    {
        base.Configure(builder);
        builder.HasData(
            new SchoolStage
            {
                Id = SchoolStageIds.Kindergarten,
                BackendName = nameof(SchoolStageIds.Kindergarten),
                NameEn = "Kindergarten",
                NameAr = "رياض أطفال",
                DisplayOrder = 1
            },
            new SchoolStage
            {
                Id = SchoolStageIds.Primary,
                BackendName = nameof(SchoolStageIds.Primary),
                NameEn = "Primary",
                NameAr = "ابتدائي",
                DisplayOrder = 2
            },
            new SchoolStage
            {
                Id = SchoolStageIds.Preparatory,
                BackendName = nameof(SchoolStageIds.Preparatory),
                NameEn = "Preparatory",
                NameAr = "إعدادي",
                DisplayOrder = 3
            },
            new SchoolStage
            {
                Id = SchoolStageIds.Secondary,
                BackendName = nameof(SchoolStageIds.Secondary),
                NameEn = "Secondary",
                NameAr = "ثانوي",
                DisplayOrder = 4
            }
        );
    }
}
