using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Domain.Configurations.Entities.Lookups;

public class StudyTypeConfiguration : LookupBaseConfiguration<StudyType>
{
    public override void Configure(EntityTypeBuilder<StudyType> builder)
    {
        builder.HasData(
            new StudyType
            {
                Id = StudyTypeIds.Regular,
                BackendName = nameof(StudyTypeIds.Regular),
                NameEn = "Regular Study",
                NameAr = "دراسة نظامية",
                DisplayOrder = 1
            },
            new StudyType
            {
                Id = StudyTypeIds.DistanceLearning,
                BackendName = nameof(StudyTypeIds.DistanceLearning),
                NameEn = "Distance Learning",
                NameAr = "تعليم عن بعد",
                DisplayOrder = 2
            },
            new StudyType
            {
                Id = StudyTypeIds.Affiliation,
                BackendName = nameof(StudyTypeIds.Affiliation),
                NameEn = "Affiliation Study",
                NameAr = "دراسة انتساب",
                DisplayOrder = 3
            }
        );
        base.Configure(builder);
    }
}
