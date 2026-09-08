using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Interview;

namespace Tawtheef.Infrastructure.Configurations.Entities.Interview;

public sealed class InterviewTemplateConfiguration : BaseEntityConfiguration<InterviewTemplate>
{
    public override void Configure(EntityTypeBuilder<InterviewTemplate> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.TitleAr).IsRequired().HasMaxLength(200);
        builder.Property(x => x.TitleEn).HasMaxLength(200);
        builder.Property(x => x.IsActive).HasDefaultValue(true);

        builder.HasOne(x => x.JobTitle)
            .WithMany()
            .HasForeignKey(x => x.JobTitleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Department)
            .WithMany()
            .HasForeignKey(x => x.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.OrganizationScope)
            .WithMany()
            .HasForeignKey(x => x.OrganizationScopeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
