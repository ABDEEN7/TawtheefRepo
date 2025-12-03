using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Infrastructure.Configurations.Entities;

public class SkillRequirementTypeConfiguration : IEntityTypeConfiguration<SkillRequirementType>
{
    public void Configure(EntityTypeBuilder<SkillRequirementType> builder)
    {
        builder.ToTable(nameof(SkillRequirementType), Schemas.Lookup);
    }
}
