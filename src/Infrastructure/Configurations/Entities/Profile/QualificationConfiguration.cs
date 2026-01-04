using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Applicant;

namespace Tawtheef.Infrastructure.Configurations.Entities.Profile;

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
            .WithMany(up => up.Qualifications)
            .HasForeignKey(q=>q.UserProfileId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(q => q.Major)
            .WithMany()
            .HasForeignKey(q => q.MajorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(q => q.SubMajor)
            .WithMany()
            .HasForeignKey(q => q.SubMajorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
