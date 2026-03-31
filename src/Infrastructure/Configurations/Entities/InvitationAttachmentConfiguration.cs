using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Recruitment;

namespace Tawtheef.Infrastructure.Configurations.Entities;

public class InvitationAttachmentConfiguration : IEntityTypeConfiguration<InvitationAttachment>
{
    public void Configure(EntityTypeBuilder<InvitationAttachment> builder)
    {
        builder.HasOne(x => x.Invitation)
            .WithMany(i => i.Attachments)
            .HasForeignKey(x => x.InvitationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.JobRequiredAttachment)
            .WithMany()
            .HasForeignKey(x => x.JobRequiredAttachmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Resource)
            .WithMany()
            .HasForeignKey(x => x.ResourceId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
