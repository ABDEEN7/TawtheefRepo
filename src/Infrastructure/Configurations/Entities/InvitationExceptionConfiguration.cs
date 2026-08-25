using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Recruitment;

namespace Tawtheef.Infrastructure.Configurations.Entities;

public sealed class InvitationExceptionConfiguration : IEntityTypeConfiguration<InvitationException>
{
    public void Configure(EntityTypeBuilder<InvitationException> builder)
    {
        builder.Property(exception => exception.Reason)
            .IsRequired()
            .HasMaxLength(InvitationException.ReasonMaxLength);

        builder.Property(exception => exception.CancellationReason)
            .HasMaxLength(InvitationException.ReasonMaxLength);

        builder.Property(exception => exception.Status)
            .IsRequired()
            .HasMaxLength(50)
            .HasConversion<string>()
            .HasDefaultValue(InvitationExceptionStatus.ReadyToSend)
            .HasSentinel(0);

        builder.HasOne(exception => exception.Job)
            .WithMany()
            .HasForeignKey(exception => exception.JobId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(exception => exception.Applicant)
            .WithMany()
            .HasForeignKey(exception => exception.ApplicantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(exception => exception.Invitation)
            .WithMany()
            .HasForeignKey(exception => exception.InvitationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(exception => exception.ProofResource)
            .WithMany()
            .HasForeignKey(exception => exception.ProofResourceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(exception => exception.JobId);
        builder.HasIndex(exception => exception.ApplicantId);
        builder.HasIndex(exception => exception.InvitationId);

        builder.HasIndex(exception => new { exception.ApplicantId, exception.JobId })
            .IsUnique()
            .HasDatabaseName("IX_InvitationException_ApplicantId_JobId_InFlight")
            .HasFilter("[Status] IN (N'ReadyToSend', N'InvitationSent') AND [IsDeleted] = 0");
    }
}
