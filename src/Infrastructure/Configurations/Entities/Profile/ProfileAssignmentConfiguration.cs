using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Recruitment;

namespace Tawtheef.Infrastructure.Configurations.Entities.Profile;

public class ProfileAssignmentConfiguration :  IEntityTypeConfiguration<ProfileAssignment>
{

    public void Configure(EntityTypeBuilder<ProfileAssignment> builder)
    {
        builder.HasOne(pa => pa.UserProfile)
            .WithMany(up => up.ProfileAssignments)
            .HasForeignKey(pa => pa.UserProfileId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(pa => pa.Employee)
            .WithMany(e => e.ProfileAssignments)
            .HasForeignKey(pa => pa.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
