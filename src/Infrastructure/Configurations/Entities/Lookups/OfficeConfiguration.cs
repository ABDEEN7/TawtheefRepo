using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Tawtheef.Infrastructure.Configurations.Entities.Lookups;

public class OfficeConfiguration : LookupBaseConfiguration<Office>
{
    // public static readonly Guid Qatar = Guid.Parse("f58c33c6-5d13-4d29-8c81-7b52a56a62ab");
    // public static readonly Guid Egypt = Guid.Parse("8f81a090-2e40-413b-bc37-ccba4b6a3b79");
    public override void Configure(EntityTypeBuilder<Office> builder)
    {
        base.Configure(builder);

        builder.Property(o => o.Code).HasMaxLength(50).IsRequired();

        builder.HasIndex(o => o.Code).IsUnique();

        builder.HasOne(o => o.Country)
            .WithMany()
            .HasForeignKey(o => o.CountryId)
            .OnDelete(DeleteBehavior.Restrict);

        // builder.HasData(
        //     new Office
        //     {
        //         Id = Guid.Parse("9f335702-71a2-4188-b8f0-d2a52d0d6df6"),
        //         BackendName = "DohaMainOffice",
        //         NameEn = "Doha Main Office",
        //         NameAr = "المكتب الرئيسي - الدوحة",
        //         DescriptionEn = "Primary accredited office located in Doha",
        //         DescriptionAr = "المكتب المعتمد الرئيسي في الدوحة",
        //         CountryId = Qatar,
        //         Code = "QA-DOH-01",
        //         DisplayOrder = 1
        //     },
        //     new Office
        //     {
        //         Id = Guid.Parse("3e6dfb62-96f4-4d95-96c5-38a8a454a269"),
        //         BackendName = "CairoRegionalOffice",
        //         NameEn = "Cairo Regional Office",
        //         NameAr = "المكتب الإقليمي - القاهرة",
        //         DescriptionEn = "Accredited regional office serving applicants outside Qatar",
        //         DescriptionAr = "مكتب معتمد إقليمي لخدمة المتقدمين خارج قطر",
        //         CountryId = Egypt,
        //         Code = "EG-CAI-01",
        //         DisplayOrder = 2
        //     }
        // );
    }
}
