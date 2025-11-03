using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Domain.Configurations.Entities.Lookups;

public class RatingGradeConfiguration : LookupBaseConfiguration<RatingGrade>
{
    public override void Configure(EntityTypeBuilder<RatingGrade> builder)
    {
        builder.HasData(
            new RatingGrade
            {
                Id = RatingGradeIds.Excellent,
                BackendName = nameof(RatingGradeIds.Excellent),
                NameEn = "Excellent",
                NameAr = "ممتاز",
                DisplayOrder = 1
            },
            new RatingGrade
            {
                Id = RatingGradeIds.VeryGood,
                BackendName = nameof(RatingGradeIds.VeryGood),
                NameEn = "Very Good",
                NameAr = "جيد جدًا",
                DisplayOrder = 2
            },
            new RatingGrade
            {
                Id = RatingGradeIds.Good,
                BackendName = nameof(RatingGradeIds.Good),
                NameEn = "Good",
                NameAr = "جيد",
                DisplayOrder = 3
            },
            new RatingGrade
            {
                Id = RatingGradeIds.Acceptable,
                BackendName = nameof(RatingGradeIds.Acceptable),
                NameEn = "Acceptable",
                NameAr = "مقبول",
                DisplayOrder = 4
            }
        );
        base.Configure(builder);
    }
}
