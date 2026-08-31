using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Exams;

namespace Tawtheef.Infrastructure.Configurations.Entities.Exams;

public class TestSlotStaffConfiguration : BaseEntityConfiguration<TestSlotStaff>
{
    public override void Configure(EntityTypeBuilder<TestSlotStaff> builder)
    {
        base.Configure(builder);

        builder.HasOne(x => x.TestSlot)
            .WithMany()
            .HasForeignKey(x => x.TestSlotId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Role)
            .WithMany()
            .HasForeignKey(x => x.RoleId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

