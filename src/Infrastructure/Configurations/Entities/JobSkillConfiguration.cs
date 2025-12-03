using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;

namespace Tawtheef.Infrastructure.Configurations.Entities;

public class JobSkillConfiguration : IEntityTypeConfiguration<JobSkill>
{
    public void Configure(EntityTypeBuilder<JobSkill> builder)
    {
        builder.ToTable(nameof(JobSkill), Schemas.Hr);

        builder.Property(s => s.ShowToApplicants).IsRequired();

        // العلاقة مع Skill المرجعية
        builder.HasOne(s => s.Skill)
            .WithMany()
            .HasForeignKey(s => s.SkillId)
            .OnDelete(DeleteBehavior.Restrict);

        // فهارس
        builder.HasIndex(s => s.JobId);
        builder.HasIndex(s => s.SkillId);
    }
}
