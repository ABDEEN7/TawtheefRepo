using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Interview;

namespace Tawtheef.Infrastructure.Configurations.Entities.Interview;

public sealed class InterviewCommitteeMemberConfiguration : BaseEntityConfiguration<InterviewCommitteeMember>
{
    public override void Configure(EntityTypeBuilder<InterviewCommitteeMember> builder)
    {
        base.Configure(builder);

        builder.HasOne(x => x.InterviewCommittee)
            .WithMany(c => c.Members)
            .HasForeignKey(x => x.InterviewCommitteeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.MemberUser)
            .WithMany()
            .HasForeignKey(x => x.MemberUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Exactly one active Chair per committee.
        builder.HasIndex(x => x.InterviewCommitteeId)
            .IsUnique()
            .HasFilter("[Role] = 1 AND [IsActive] = 1")
            .HasDatabaseName("UX_Committee_Chair");

        // A user sits on a committee once (while active).
        builder.HasIndex(x => new { x.InterviewCommitteeId, x.MemberUserId })
            .IsUnique()
            .HasFilter("[IsActive] = 1")
            .HasDatabaseName("UX_Committee_Member");
    }
}
