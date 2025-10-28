using System.Linq.Expressions;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Serilog;
using Tawtheef.Application.Common.Interfaces;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Common.Interfaces;
using Tawtheef.Domain.Entities;
using Tawtheef.Domain.Entities.Applicant;
using Tawtheef.Domain.Entities.Auth;
using Tawtheef.Domain.Entities.Logger;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Notification;
using Tawtheef.Domain.Entities.Users;
using Tawtheef.Infrastructure.Data.Interceptors;
using EmailQueue = Tawtheef.Infrastructure.Services.NotificationServices.EmailQueue;

namespace Tawtheef.Infrastructure.Data;

public class TawtheefDbContext(DbContextOptions<TawtheefDbContext> options,
    ILogger logger, TimeProvider time)
    : IdentityDbContext<User, IdentityRole<Guid>, Guid>(options), ITawtheefDbContext
{
    // Base Table
    public DbSet<EntityLog> EntityLogs { get; set; }

    // Lookup Tables
    public DbSet<UserType> UserTypes { get; set; }
    public DbSet<NotificationType> NotificationTypes { get; set; }
    public DbSet<CandidateType> CandidateTypes { get; set; }
    public DbSet<Gender> Genders { get; set; }
    public DbSet<Language> Languages { get; set; }
    public DbSet<LanguageLevel> LanguageLevels { get; set; }
    public DbSet<MaritalStatus> MaritalStatuses { get; set; }
    public DbSet<QualificationLevel> QualificationLevels { get; set; }
    public DbSet<RatingGrade> RatingGrades { get; set; }
    public DbSet<Religion> Religions { get; set; }
    public DbSet<StudyType> StudyTypes { get; set; }
    public DbSet<TargetEntity> TargetEntities { get; set; }
    public DbSet<Country> Countries { get; set; }
    public DbSet<Major> Majors { get; set; }
    public DbSet<Nationality> Nationalities { get; set; }
    public DbSet<SkillType> SkillTypes { get; set; }
    public DbSet<University> Universities { get; set; }

    // Base User Tables
    public DbSet<AdminUser> Admins { get; set; }
    public DbSet<EmployeeUser> Employees { get; set; }
    public DbSet<ApplicantUser> Applicants { get; set; }
    public DbSet<UserProfile> UserProfiles { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<OTPRequest> OTPRequests { get; set; }
    
    // Applicant Tables
    public DbSet<Experience> Experiences { get; set; }
    public DbSet<Qualification> Qualifications { get; set; }
    public DbSet<ApplicantSkill> ApplicantSkills { get; set; }
    public DbSet<LanguageProficiency> LanguageProficiencies { get; set; }
    public DbSet<ResidenceAddress> ResidenceAddresses { get; set; }
    public DbSet<TrainingCourse> TrainingCourses { get; set; }
    public DbSet<AdditionalAttachmentApplicant> AdditionalAttachmentApplicants { get; set; }
    
    // Notification Tables
    public DbSet<EmailQueue> EmailQueues { get; set; }
    public DbSet<EmailTemplate> EmailTemplates { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    
    public DbSet<Resource> Resources { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // ensure client-side evaluation works properly
        optionsBuilder.ConfigureWarnings(warnings => 
            warnings.Ignore(CoreEventId.RowLimitingOperationWithoutOrderByWarning))
            .AddInterceptors(new SlowQueryInterceptor(logger, 300));
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply configurations
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TawtheefDbContext).Assembly);

        // Configure soft delete globally
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var clrType = entityType.ClrType;

            if (!typeof(ISoftDelete).IsAssignableFrom(clrType)) continue;
            if (entityType.BaseType != null) continue;

            var parameter = Expression.Parameter(clrType, "e");
            var property = Expression.Property(parameter, nameof(ISoftDelete.IsDeleted));
            var condition = Expression.Equal(property, Expression.Constant(false));
            var lambda = Expression.Lambda(condition, parameter);

            modelBuilder.Entity(clrType).HasQueryFilter(lambda);

            if (!typeof(BaseEntity).IsAssignableFrom(clrType)) continue;
            if (clrType == typeof(User)) continue;
            var entity = modelBuilder.Entity(clrType);

            entity.HasOne(typeof(User), nameof(BaseEntity.CreatedBy))
                .WithMany()
                .HasForeignKey(nameof(BaseEntity.CreatedById))
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(typeof(User), nameof(BaseEntity.UpdatedBy))
                .WithMany()
                .HasForeignKey(nameof(BaseEntity.UpdatedById))
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(typeof(User), nameof(BaseEntity.DeletedBy))
                .WithMany()
                .HasForeignKey(nameof(BaseEntity.DeletedById))
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
    {
        // Get current user ID (you'll need to inject IHttpContextAccessor or similar)
        var currentUserId = GetCurrentUserId();
        var now = time.GetLocalNow().DateTime;

        foreach (var entry in ChangeTracker.Entries<IBaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedDate = now;
                    entry.Entity.CreatedById = currentUserId;
                    // Ensure new entities aren't marked as deleted
                    entry.Entity.IsDeleted = false;
                    break;

                case EntityState.Modified:
                    entry.Entity.UpdatedDate = now;
                    entry.Entity.UpdatedById = currentUserId;
                    break;

                case EntityState.Deleted when !entry.Entity.IsDeleted:
                    // Convert hard delete to softly delete
                    entry.State = EntityState.Modified;
                    entry.Entity.IsDeleted = true;
                    entry.Entity.DeletedDate = now;
                    entry.Entity.DeletedById = currentUserId;
                    break;
            }
        }

        var result = await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return result;
    }
    private Guid? GetCurrentUserId()
    { 
        var userIdClaim = this.GetService<IHttpContextAccessor>().HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);
        return userIdClaim != null ? Guid.Parse(userIdClaim.Value) : null;
    }
    public override int SaveChanges()
    {
        return SaveChangesAsync().GetAwaiter().GetResult();
    }
}
