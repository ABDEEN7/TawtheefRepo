using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;

namespace Tawtheef.Infrastructure.Configurations.Entities.Job;

public class JobSkillConfiguration : IEntityTypeConfiguration<JobSkill>
{
    public void Configure(EntityTypeBuilder<JobSkill> builder)
    {
        builder.HasOne(s => s.Skill)
            .WithMany()
            .HasForeignKey(s => s.SkillId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(s => s.JobId);
        builder.HasIndex(s => s.SkillId);
    }
}
