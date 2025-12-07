using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Infrastructure.Configurations.Entities.Lookups;

public class RatingGradeConfiguration : LookupBaseConfiguration<RatingGrade>
{
    public override void Configure(EntityTypeBuilder<RatingGrade> builder)
    {
        base.Configure(builder);
        builder.HasData(
            new RatingGrade
            {
                Id = RatingGradeIds.Expert,
                BackendName = nameof(RatingGradeIds.Expert),
                NameEn = "Expert",
                NameAr = "خبير",
                DisplayOrder = 1
            },
            new RatingGrade
            {
                Id = RatingGradeIds.Advanced,
                BackendName = nameof(RatingGradeIds.Advanced),
                NameEn = "Advanced",
                NameAr = "متقدم",
                DisplayOrder = 2
            },
            new RatingGrade
            {
                Id = RatingGradeIds.Intermediate,
                BackendName = nameof(RatingGradeIds.Intermediate),
                NameEn = "Intermediate",
                NameAr = "متوسط",
                DisplayOrder = 3
            },
            new RatingGrade
            {
                Id = RatingGradeIds.Basic,
                BackendName = nameof(RatingGradeIds.Basic),
                NameEn = "Basic",
                NameAr = "أساسي",
                DisplayOrder = 4
            }
        );
    }
}
