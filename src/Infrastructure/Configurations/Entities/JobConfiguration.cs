using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;

namespace Tawtheef.Infrastructure.Configurations.Entities;

public class JobConfiguration : IEntityTypeConfiguration<Job>
{
    public void Configure(EntityTypeBuilder<Job> builder)
    {
        builder.ToTable(nameof(Job), Schemas.Hr);

        // ---------------------------------
        // BASIC RULES
        // ---------------------------------
        builder.Property(j => j.Title).IsRequired();
        builder.Property(j => j.Description).HasMaxLength(2000).IsRequired();
        builder.Property(j => j.Benefits).HasMaxLength(2000).IsRequired();
        builder.Property(j => j.Overview).HasMaxLength(4000);
        builder.Property(j => j.QualificationsDescription).HasMaxLength(4000);

        // ÇáÎÕÇÆÕ ÇáÌÏíÏÉ ÍÓÈ BRD
        builder.Property(j => j.MinimumExperienceYears).IsRequired();
        builder.Property(j => j.MinimumAge).IsRequired();
        builder.Property(j => j.MaximumAge).IsRequired();

        // ---------------------------------
        // ONE-TO-MANY LOOKUPS
        // ---------------------------------
        builder.HasOne(j => j.Sector)
            .WithMany()
            .HasForeignKey(j => j.SectorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(j => j.Management)
            .WithMany()
            .HasForeignKey(j => j.ManagementId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(j => j.RequestingDepartment)
            .WithMany()
            .HasForeignKey(j => j.RequestingDepartmentId)
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

        builder.HasOne(j => j.Status)
            .WithMany()
            .HasForeignKey(j => j.StatusId)
            .OnDelete(DeleteBehavior.Restrict);

        // ---------------------------------
        // JOB QUOTA (ONE-TO-ONE)
        // ---------------------------------
        builder.HasOne(j => j.Quota)
           .WithOne(q => q.Job)
           .HasForeignKey<JobQuota>(q => q.JobId) // Foreign key is in JobQuota
           .OnDelete(DeleteBehavior.Restrict);

        // ---------------------------------
        // COLLECTION NAVIGATIONS
        // ---------------------------------
        builder.HasMany(j => j.Invitations)
            .WithOne(i => i.Job)
            .HasForeignKey(i => i.JobId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(j => j.Degrees)
            .WithOne(d => d.Job)
            .HasForeignKey(d => d.JobId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(j => j.Conditions)
            .WithOne(c => c.Job)
            .HasForeignKey(c => c.JobId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(j => j.Skills)
            .WithOne(s => s.Job)
            .HasForeignKey(s => s.JobId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(j => j.Responsibilities)
            .WithOne(r => r.Job)
            .HasForeignKey(r => r.JobId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(j => j.RequiredAttachments)
            .WithOne(a => a.Job)
            .HasForeignKey(a => a.JobId)
            .OnDelete(DeleteBehavior.Restrict);

        // ---------------------------------
        // INDEXES
        // ---------------------------------
        builder.HasIndex(j => j.StatusId);
        builder.HasIndex(j => j.SectorId);
        builder.HasIndex(j => j.ManagementId);
        builder.HasIndex(j => j.RequestingDepartmentId);
        builder.HasIndex(j => j.MajorId);
        builder.HasIndex(j => j.SubMajorId);
        builder.HasIndex(j => j.WorkLocationId);
        builder.HasIndex(j => j.JobCategoryId);
        builder.HasIndex(j => j.WorkTypeId);
        builder.HasIndex(j => new { j.IsDeleted, j.Deadline });
        builder.HasIndex(j => new { j.IsDeleted, j.PublishAt });

        // ÅÖÇÝÉ ÝåÑÓ áãäÚ ÇáÊßÑÇÑ ÍÓÈ BRD
        builder.HasIndex(j => new {
            j.Title,
            j.RequestingDepartmentId,
            j.JobCategoryId,
            j.MajorId
        })
        .IsUnique()
        .HasFilter($"[{nameof(EventEntity.IsDeleted)}] = 0");
    }
}
