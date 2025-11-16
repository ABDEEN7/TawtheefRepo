using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Recruitment;

namespace Tawtheef.Infrastructure.Configurations.Entities;

public class InvitationConfiguration : IEntityTypeConfiguration<Invitation>
{
    public virtual void Configure(EntityTypeBuilder<Invitation> builder)
    {

        // Invitation → InvitationStatus
        builder
            .HasOne(i => i.InvitationStatus)
            .WithMany()
            .HasForeignKey(i => i.InvitationStatusId)
            .OnDelete(DeleteBehavior.Restrict);

        // Invitation → HistoryInvitation (one-to-many)
        builder
             .HasMany(i => i.History)
            .WithOne(h => h.Invitation)
            .HasForeignKey(h => h.InvitationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
