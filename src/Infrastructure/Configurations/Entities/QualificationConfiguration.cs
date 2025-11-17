using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Applicant;

namespace Tawtheef.Infrastructure.Configurations.Entities;

public class QualificationConfiguration : IEntityTypeConfiguration<Qualification>
{
    public virtual void Configure(EntityTypeBuilder<Qualification> builder)
    {
        builder
            .HasOne(q => q.University)
            .WithMany()
            .HasForeignKey(q => q.UniversityId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder
            .HasOne(q=>q.UserProfile)
            .WithMany()
            .HasForeignKey(q=>q.UserProfileId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
