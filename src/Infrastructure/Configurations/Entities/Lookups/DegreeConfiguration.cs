using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Infrastructure.Configurations.Entities.Lookups;

public class DegreeConfiguration : LookupBaseConfiguration<Degree>
{
    public override void Configure(EntityTypeBuilder<Degree> builder)
    {
        base.Configure(builder);
        builder.HasData(
            new Degree
            {
                Id = DegreeIds.Doctorate,
                BackendName = nameof(DegreeIds.Doctorate),
                NameAr = "دكتوراه",
                NameEn = "PhD",
                DescriptionAr = "دكتوراه",
                DescriptionEn = "Doctorate"
            },
            new Degree
            {
                Id = DegreeIds.Master,
                BackendName = nameof(DegreeIds.Master),
                NameAr = "ماجستير",
                NameEn = "Master's",
                DescriptionAr = "ماجستير",
                DescriptionEn = "Master"
            },
            new Degree
            {
                Id = DegreeIds.Bachelor,
                BackendName = nameof(DegreeIds.Bachelor),
                NameAr = "بكالوريوس",
                NameEn = "Bachelor's",
                DescriptionAr = "بكالوريوس",
                DescriptionEn = "Bachelor"
            },
            new Degree
            {
                Id = DegreeIds.Diploma,
                BackendName = nameof(DegreeIds.Diploma),
                NameAr = "دبلوم",
                NameEn = "Diploma",
                DescriptionAr = "دبلوم",
                DescriptionEn = "Diploma"
            },
            new Degree
            {
                Id = DegreeIds.Secondary,
                BackendName = nameof(DegreeIds.Secondary),
                NameAr = "ثانوي",
                NameEn = "Secondary",
                DescriptionAr = "ثانوي",
                DescriptionEn = "Secondary"
            },
            new Degree
            {
                Id = DegreeIds.Preparatory,
                BackendName = nameof(DegreeIds.Preparatory),
                NameAr = "إعدادي",
                NameEn = "Preparatory",
                DescriptionAr = "إعدادي",
                DescriptionEn = "Preparatory"
            },
            new Degree
            {
                Id = DegreeIds.Elementary,
                BackendName = nameof(DegreeIds.Elementary),
                NameAr = "ابتدائي",
                NameEn = "Elementary",
                DescriptionAr = "ابتدائي",
                DescriptionEn = "Elementary"
            }
        );
    }
}
