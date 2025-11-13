using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Infrastructure.Configurations.Entities.Lookups;

public class GenderConfiguration : LookupBaseConfiguration<Gender>
{
    public override void Configure(EntityTypeBuilder<Gender> builder)
    {
        base.Configure(builder);
        builder.HasData(
            new Gender
            {
                Id = GenderIds.Male,
                BackendName = nameof(GenderIds.Male),
                NameAr = "ذكر",
                NameEn = "Male",
                DescriptionAr = "ذكر",
                DescriptionEn = "Male"
            },
            new Gender
            {
                Id = GenderIds.Female,
                BackendName = nameof(GenderIds.Female),
                NameAr = "أنثى",
                NameEn = "Female",
                DescriptionAr = "أنثى",
                DescriptionEn = "Female"
            }
        );
    }
}
