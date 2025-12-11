using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Infrastructure.Configurations.Entities.Lookups;

public class ManagementConfiguration : LookupBaseConfiguration<Management>
{
    public override void Configure(EntityTypeBuilder<Management> builder)
    {
        base.Configure(builder);
        builder.HasOne(m => m.Sector)
            .WithMany()
            .HasForeignKey(m => m.SectorId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasData(
            new Management
            {
                Id = ManagementIds.Minister,
                SectorId = SectorIds.DeputyMinisterSector,
                BackendName = nameof(ManagementIds.Minister),
                NameEn = "Minister",
                NameAr = "الوزير",
                DisplayOrder = 1
            },
            new Management
            {
                Id = ManagementIds.TrainingCenter,
                SectorId = SectorIds.GeneralEducationSector,
                BackendName = nameof(ManagementIds.TrainingCenter),
                NameEn = "Training Center",
                NameAr = "مركز التدريب",
                DisplayOrder = 2
            }
        );
    }
}
