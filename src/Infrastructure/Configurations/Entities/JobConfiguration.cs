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
        
        // Basic Property Configurations
        builder.Property(j => j.Title)
            .IsRequired()
            .HasMaxLength(200);
            
        builder.Property(j => j.Vacancies)
            .IsRequired();
            
        builder.Property(j => j.Deadline)
            .IsRequired();
            
        builder.Property(j => j.Description)
            .HasMaxLength(2000);
            
        builder.Property(j => j.Benefits)
            .HasMaxLength(2000);

        // Navigation Property Configurations
        builder.HasOne(j => j.RequestingDepartment)
            .WithMany()
            .HasForeignKey(j => j.RequestingDepartmentId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(j => j.JobCategory)
            .WithMany()
            .HasForeignKey(j => j.JobCategoryId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(j => j.Gender)
            .WithMany()
            .HasForeignKey(j => j.GenderId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(j => j.WorkLocation)
            .WithMany()
            .HasForeignKey(j => j.WorkLocationId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(j => j.Major)
            .WithMany()
            .HasForeignKey(j => j.MajorId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(j => j.WorkType)
            .WithMany()
            .HasForeignKey(j => j.WorkTypeId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(j => j.Status)
            .WithMany()
            .HasForeignKey(j => j.StatusId)
            .OnDelete(DeleteBehavior.Restrict);

        // One-to-One with JobQuotas
        builder.HasOne(j => j.Quota)
            .WithOne(q => q.Job)
            .HasForeignKey<JobQuota>(q => q.JobId)
            .OnDelete(DeleteBehavior.Cascade);
            
        // One-to-Many Relationships
        builder.HasMany(j => j.Skills)
            .WithOne(s => s.Job)
            .HasForeignKey(s => s.JobId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasMany(j => j.Conditions)
            .WithOne(c => c.Job)
            .HasForeignKey(c => c.JobId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasMany(j => j.Degrees)
            .WithOne(d => d.Job)
            .HasForeignKey(d => d.JobId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes for better performance
        builder.HasIndex(j => j.StatusId);
        builder.HasIndex(j => j.RequestingDepartmentId);
        builder.HasIndex(j => j.Deadline);
        builder.HasIndex(j => new { j.IsDeleted, j.Deadline });
    }
}
