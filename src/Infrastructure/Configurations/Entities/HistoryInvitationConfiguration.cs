using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Recruitment;

namespace Tawtheef.Infrastructure.Configurations.Entities;

public class HistoryInvitationConfiguration : IEntityTypeConfiguration<HistoryInvitation>
{
    public virtual void Configure(EntityTypeBuilder<HistoryInvitation> builder)
    {
        // HistoryInvitation → InvitationStatus
        builder
            .HasOne(h => h.InvitationStatus)
            .WithMany()
            .HasForeignKey(h => h.InvitationStatusId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
