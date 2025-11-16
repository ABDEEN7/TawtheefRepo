using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Applicant;

namespace Tawtheef.Infrastructure.Configurations.Entities;

public class QualificationConfiguration : IEntityTypeConfiguration<Qualification>
{
    public virtual void Configure(EntityTypeBuilder<Qualification> builder)
    {
        builder
            .HasOne(u => u.University)
            .WithMany()
            .HasForeignKey(u => u.UniversityId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
