using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Infrastructure.Configurations.Entities.Lookups;

public class DifficultyLevelConfiguration : LookupBaseConfiguration<DifficultyLevel>
{
    public override void Configure(EntityTypeBuilder<DifficultyLevel> builder)
    {
        base.Configure(builder);

        builder.HasData(
            new DifficultyLevel
            {
                Id = DifficultyLevelIds.EASY,
                BackendName = "EASY",
                NameEn = "Easy",
                NameAr = "سهل",
                DisplayOrder = 1
            },
            new DifficultyLevel
            {
                Id = DifficultyLevelIds.MEDIUM,
                BackendName = "MEDIUM",
                NameEn = "Medium",
                NameAr = "متوسط",
                DisplayOrder = 2
            },
            new DifficultyLevel
            {
                Id = DifficultyLevelIds.HARD,
                BackendName = "HARD",
                NameEn = "Hard",
                NameAr = "صعب",
                DisplayOrder = 3
            }
        );
    }
}
