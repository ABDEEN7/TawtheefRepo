using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Infrastructure.Configurations.Entities.Lookups;

public class WorkTypeConfiguration : LookupBaseConfiguration<WorkType>
{
    public override void Configure(EntityTypeBuilder<WorkType> builder)
    {
        base.Configure(builder);
        builder.HasData(
            new WorkType
            {
                Id = WorkTypeIds.FullTime,
                BackendName = nameof(WorkTypeIds.FullTime),
                NameEn = "Full-Time",
                NameAr = "دوام كامل",
                DescriptionAr = "وظيفة بدوام كامل",
                DescriptionEn = "Full-time job",
                DisplayOrder = 1
            },
            new WorkType
            {
                Id = WorkTypeIds.PartTime,
                BackendName = nameof(WorkTypeIds.PartTime),
                NameEn = "Part-Time",
                NameAr = "دوام جزئي",
                DescriptionAr = "وظيفة بدوام جزئي",
                DescriptionEn = "Part-time job",
                DisplayOrder = 2
            }
        );
    }
}
