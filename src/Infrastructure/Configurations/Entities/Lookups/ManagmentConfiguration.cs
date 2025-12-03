using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Infrastructure.Configurations.Entities.Lookups;

public class ManagmentConfiguration : LookupBaseConfiguration<Managment>
{
    public override void Configure(EntityTypeBuilder<Managment> builder)
    {
        base.Configure(builder);
        builder.HasData(
            new Managment
            {
                Id = ManagmentIds.Minister,
                SectorId = SectorIds.DeputyMinisterSector,
                BackendName = nameof(ManagmentIds.Minister),
                NameEn = "Minister",
                NameAr = "الوزير",
                DisplayOrder = 1
            },
            new Managment
            {
                Id = ManagmentIds.TrainingCenter,
                SectorId = SectorIds.GeneralEducationSector,
                BackendName = nameof(ManagmentIds.TrainingCenter),
                NameEn = "Training Center",
                NameAr = "مركز التدريب",
                DisplayOrder = 2
            }
        );
        builder.HasOne(m => m.Sector)
               .WithMany()
               .HasForeignKey(m => m.SectorId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
