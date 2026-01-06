using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;

namespace Tawtheef.Infrastructure.Configurations.Entities.Job;

public class JobConfiguration : IEntityTypeConfiguration<Domain.Entities.Recruitment.Job>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Recruitment.Job> builder)
    {
        builder.HasQueryFilter(j => !j.IsDeleted);

        builder.HasOne(j => j.Sector)
            .WithMany()
            .HasForeignKey(j => j.SectorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(j => j.Management)
            .WithMany()
            .HasForeignKey(j => j.ManagementId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(j => j.Department)
            .WithMany()
            .HasForeignKey(j => j.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(j => j.JobCategory)
            .WithMany()
            .HasForeignKey(j => j.JobCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(j => j.Gender)
            .WithMany()
            .HasForeignKey(j => j.GenderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(j => j.WorkLocation)
            .WithMany()
            .HasForeignKey(j => j.WorkLocationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(j => j.Major)
            .WithMany()
            .HasForeignKey(j => j.MajorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(j => j.SubMajor)
            .WithMany()
            .HasForeignKey(j => j.SubMajorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(j => j.WorkType)
            .WithMany()
            .HasForeignKey(j => j.WorkTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(j => j.JobStatus)
            .WithMany()
            .HasForeignKey(j => j.JobStatusId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(j => j.JobDegrees)
            .WithOne(d => d.Job)
            .HasForeignKey(d => d.JobId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(j => j.JobConditions)
            .WithOne(c => c.Job)
            .HasForeignKey(c => c.JobId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(j => j.JobSkills)
            .WithOne(s => s.Job)
            .HasForeignKey(s => s.JobId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(j => j.JobResponsibilities)
            .WithOne(r => r.Job)
            .HasForeignKey(r => r.JobId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(j => j.JobRequiredAttachments)
            .WithOne(a => a.Job)
            .HasForeignKey(a => a.JobId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(j => j.Invitations)
            .WithOne(i => i.Job)
            .HasForeignKey(i => i.JobId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(j => j.TabReviewNotes)
               .WithOne(n => n.Job)
               .HasForeignKey(n => n.JobId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(j => j.JobPoints)
       .WithOne(p => p.Job)
       .HasForeignKey<JobPointsMain>(p => p.JobId)
       .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(j => new
        {
            j.TitleAr,
            j.DepartmentId,
            j.JobCategoryId,
            j.SubMajorId
        })
        .IsUnique()
        .HasFilter($"[{nameof(EventEntity.IsDeleted)}] = 0")
        .HasDatabaseName("IX_Job_Unique_Title_Department_Category_SubMajor");

        builder.HasIndex(j => j.SectorId)
            .HasDatabaseName("IX_Job_SectorId");

        builder.HasIndex(j => j.ManagementId)
            .HasDatabaseName("IX_Job_ManagementId");

        builder.HasIndex(j => j.DepartmentId)
            .HasDatabaseName("IX_Job_DepartmentId");

        builder.HasIndex(j => j.JobStatusId)
            .HasDatabaseName("IX_Job_JobStatusId");

        builder.HasIndex(j => j.ClosingDate)
            .HasDatabaseName("IX_Job_ClosingDate");

        builder.HasIndex(j => new { j.IsDeleted, j.JobStatusId })
            .HasDatabaseName("IX_Job_Deleted_Status");

        builder.HasIndex(j => new { j.IsDeleted, j.ClosingDate })
            .HasDatabaseName("IX_Job_Deleted_ClosingDate");

        builder.HasIndex(j => new { j.JobStatusId, j.ClosingDate, j.IsDeleted })
            .HasDatabaseName("IX_Job_Status_ClosingDate_Deleted");
    }
}
