using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Domain.Configurations.Entities.Enums;

public class QualificationLevelConfiguration : LookupBaseConfiguration<QualificationLevel>
{
    public override void Configure(EntityTypeBuilder<QualificationLevel> builder)
    {
        builder.HasData(
            new QualificationLevel
            {
                Id = QualificationLevelIds.HighSchool,
                BackendName = nameof(QualificationLevelIds.HighSchool),
                NameEn = "High School",
                NameAr = "الثانوية العامة",
                DisplayOrder = 1
            },
            new QualificationLevel
            {
                Id = QualificationLevelIds.Diploma,
                BackendName = nameof(QualificationLevelIds.Diploma),
                NameEn = "Diploma",
                NameAr = "دبلوم",
                DisplayOrder = 2
            },
            new QualificationLevel
            {
                Id = QualificationLevelIds.Bachelor,
                BackendName = nameof(QualificationLevelIds.Bachelor),
                NameEn = "Bachelor",
                NameAr = "بكالوريوس",
                DisplayOrder = 3
            },
            new QualificationLevel
            {
                Id = QualificationLevelIds.Master,
                BackendName = nameof(QualificationLevelIds.Master),
                NameEn = "Master",
                NameAr = "ماجستير",
                DisplayOrder = 4
            },
            new QualificationLevel
            {
                Id = QualificationLevelIds.Doctorate,
                BackendName = nameof(QualificationLevelIds.Doctorate),
                NameEn = "Doctorate",
                NameAr = "دكتوراه",
                DisplayOrder = 5
            }
        );
        base.Configure(builder);
    }
}
