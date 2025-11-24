using System.Linq.Expressions;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Tawtheef.Application.Common.Interfaces;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Common.Interfaces;
using Tawtheef.Domain.Entities;
using Tawtheef.Domain.Entities.Applicant;
using Tawtheef.Domain.Entities.Auth;
using Tawtheef.Domain.Entities.Logger;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;
using Tawtheef.Domain.Entities.Notification;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;
using Tawtheef.Domain.Entities.Users;
using Tawtheef.Infrastructure.Data.Interceptors;
using ILogger = Serilog.ILogger;

namespace Tawtheef.Infrastructure.Data;

public class TawtheefDbContext(DbContextOptions<TawtheefDbContext> options,
    ILogger logger, TimeProvider time)
    : IdentityDbContext<
        User,
        IdentityRole<Guid>,
        Guid,
        IdentityUserClaim<Guid>,
        IdentityUserRole<Guid>,
        IdentityUserLogin<Guid>,
        IdentityRoleClaim<Guid>,
        IdentityUserToken<Guid>>(options), ITawtheefDbContext
{
    // Base Table
    public DbSet<EntityLog> EntityLog { get; set; }

    // Lookup Tables
    public DbSet<CandidateType> CandidateType { get; set; }
    public DbSet<Degree> Degree { get; set; }
    public DbSet<Department> Department { get; set; }
    public DbSet<Gender> Gender { get; set; }
    public DbSet<InvitationStatus> InvitationStatus { get; set; }
    public DbSet<JobCategory> JobCategory { get; set; }
    public DbSet<JobStatus> JobStatus { get; set; }
    public DbSet<Language> Language { get; set; }
    public DbSet<LanguageLevel> LanguageLevel { get; set; }
    public DbSet<MaritalStatus> MaritalStatus { get; set; }
    public DbSet<QualificationLevel> QualificationLevel { get; set; }
    public DbSet<RatingGrade> RatingGrade { get; set; }
    public DbSet<Religion> Religion { get; set; }
    public DbSet<Sector> Sector { get; set; }
    public DbSet<StudyType> StudyType { get; set; }
    public DbSet<TargetEntity> TargetEntity { get; set; }
    public DbSet<UserType> UserType { get; set; }
    public DbSet<WorkType> WorkType { get; set; }
    public DbSet<SponsorType> SponsorType { get; set; }
    
    public DbSet<City> City { get; set; }
    public DbSet<Country> Country { get; set; }
    public DbSet<Major> Major { get; set; }
    public DbSet<SkillType> SkillType { get; set; }
    public DbSet<University> University { get; set; }

    // Base User Tables
    public DbSet<AdminUser> Admin { get; set; }
    public DbSet<EmployeeUser> Employee { get; set; }
    public DbSet<ApplicantUser> Applicant { get; set; }
    public DbSet<UserProfile> UserProfile { get; set; }
    public DbSet<SponsorProfile> SponsorProfile { get; set; }
    public DbSet<RefreshToken> RefreshToken { get; set; }
    public DbSet<ContactVerification> ContactVerification { get; set; }
    public DbSet<UserSession> UserSession { get; set; }
    
    // Applicant Tables
    public DbSet<Experience> Experience { get; set; }
    public DbSet<Qualification> Qualification { get; set; }
    public DbSet<ProfileSkill> ApplicantSkill { get; set; }
    public DbSet<ProfileLanguage> LanguageProficiency { get; set; }
    public DbSet<ResidenceAddress> ResidenceAddress { get; set; }
    public DbSet<TrainingCourse> TrainingCourse { get; set; }
    public DbSet<ProfileAdditionalAttachment> AdditionalAttachmentApplicant { get; set; }
    
    // Recruitment Tables
    public DbSet<Job> Jobs { get; set; }
    public DbSet<Invitation> Invitations { get; set; }
    public DbSet<JobDegree> JobDegrees { get; set; }
    public DbSet<JobQuotas> JobQuotas { get; set; }
    public DbSet<JobSkill> JobSkills { get; set; }
    public DbSet<ResidentBreakdown> ResidentBreakdowns { get; set; }
    
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
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Apply configurations
        builder.ApplyConfigurationsFromAssembly(typeof(TawtheefDbContext).Assembly);

        // Configure soft delete globally
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            var clrType = entityType.ClrType;

            if (!typeof(ISoftDelete).IsAssignableFrom(clrType)) continue;
            if (entityType.BaseType != null) continue;

            var parameter = Expression.Parameter(clrType, "e");
            var property = Expression.Property(parameter, nameof(ISoftDelete.IsDeleted));
            var condition = Expression.Equal(property, Expression.Constant(false));
            var lambda = Expression.Lambda(condition, parameter);

            builder.Entity(clrType).HasQueryFilter(lambda);

            if (!typeof(BaseEntity).IsAssignableFrom(clrType)) continue;
            if (clrType == typeof(User)) continue;
            var entity = builder.Entity(clrType);
            
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
