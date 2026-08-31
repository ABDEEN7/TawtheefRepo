using System.Linq.Expressions;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Tawtheef.Application.Common.Interfaces;
using Tawtheef.Application.Common.Interfaces.Logging;
using Tawtheef.Application.Common.Interfaces.Services.Security;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Common.Interfaces;
using Tawtheef.Domain.Entities;
using Tawtheef.Domain.Entities.Applicant;
using Tawtheef.Domain.Entities.Auth;
using Tawtheef.Domain.Entities.Content;
using Tawtheef.Domain.Entities.Exams;
using Tawtheef.Domain.Entities.Kawader;
using Tawtheef.Domain.Entities.Logger;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;
using Tawtheef.Domain.Entities.MinisterOffice;
using Tawtheef.Domain.Entities.Notification;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;
using Tawtheef.Domain.Entities.Security;
using Tawtheef.Domain.Entities.Users;
using Tawtheef.Infrastructure.Data.Interceptors;

namespace Tawtheef.Infrastructure.Data;

public class TawtheefDbContext(DbContextOptions<TawtheefDbContext> options,
    IAppLogger logger,
    IIdentityFieldProtectionContext identityFieldProtectionContext)
    : IdentityDbContext<
        User,
        ApplicationRole,
        Guid,
        IdentityUserClaim<Guid>,
        IdentityUserRole<Guid>,
        IdentityUserLogin<Guid>,
        IdentityRoleClaim<Guid>,
        IdentityUserToken<Guid>>(options), ITawtheefDbContext
{
    // Base Table
    public DbSet<EntityLog> EntityLog { get; set; }
    public DbSet<ActionLog> ActionLog { get; set; }

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
    public DbSet<RatingGrade> RatingGrade { get; set; }
    public DbSet<SkillLevel> SkillLevel { get; set; }
    public DbSet<AchievementType> AchievementType { get; set; }
    public DbSet<Religion> Religion { get; set; }
    public DbSet<Management> Management { get; set; }
    public DbSet<Sector> Sector { get; set; }
    public DbSet<StudyType> StudyType { get; set; }
    public DbSet<TargetEntity> TargetEntity { get; set; }
    public DbSet<UserType> UserType { get; set; }
    public DbSet<WorkType> WorkType { get; set; }
    public DbSet<SponsorType> SponsorType { get; set; }
    public DbSet<CandidateTypeProviderLogin> CandidateTypeProviderLogin { get; set; }
    public DbSet<ProviderLogin> ProviderLogin { get; set; }
    public DbSet<Permission> Permission { get; set; }
    public DbSet<ExamInterruptionPolicy> ExamInterruptionPolicy { get; set; }
    public DbSet<ExamStatus> ExamStatus { get; set; }
    public DbSet<ExamCategoryType> ExamCategoryType { get; set; }
    public DbSet<RoomType> RoomType { get; set; }
    public DbSet<RoomStatus> RoomStatus { get; set; }
    public DbSet<TestSlotStatus> TestSlotStatus { get; set; }
    public DbSet<TestSlotStaffRole> TestSlotStaffRole { get; set; }
    public DbSet<TestSessionStatus> TestSessionStatus { get; set; }
    public DbSet<TestAttemptInterruptionStatus> TestAttemptInterruptionStatus { get; set; }
    public DbSet<TestAttemptInterruptionResolutionAction> TestAttemptInterruptionResolutionAction { get; set; }
    public DbSet<TestSessionCandidateAttendanceStatus> TestSessionCandidateAttendanceStatus { get; set; }
    public DbSet<TestSessionCandidateIdentityVerificationStatus> TestSessionCandidateIdentityVerificationStatus { get; set; }
    public DbSet<TestSessionCandidateStatus> TestSessionCandidateStatus { get; set; }
    public DbSet<TestAttemptStatus> TestAttemptStatus { get; set; }
    public DbSet<TestAttemptPartStatus> TestAttemptPartStatus { get; set; }
    public DbSet<ExamResultReportStatus> ExamResultReportStatus { get; set; }
    public DbSet<ExamResultCandidateResultStatus> ExamResultCandidateResultStatus { get; set; }
    public DbSet<ExamExemptionDecisionStatus> ExamExemptionDecisionStatus { get; set; }

    public DbSet<City> City { get; set; }
    public DbSet<Country> Country { get; set; }
    public DbSet<Major> Major { get; set; }
    public DbSet<Skill> Skill { get; set; }
    public DbSet<Office> Office { get; set; }
    public DbSet<SkillType> SkillType { get; set; }
    public DbSet<University> University { get; set; }
    public DbSet<JobTitle> JobTitle { get; set; }

    // Base User Tables
    public DbSet<AdminUser> Admin { get; set; }
    public DbSet<EmployeeUser> Employee { get; set; }
    public DbSet<ApplicantUser> Applicant { get; set; }
    public DbSet<UserProfile> UserProfile { get; set; }
    public DbSet<SponsorProfile> SponsorProfile { get; set; }
    public DbSet<RefreshToken> RefreshToken { get; set; }
    public DbSet<LoginAttempt> LoginAttempt { get; set; }
    public DbSet<ContactVerification> ContactVerification { get; set; }
    public DbSet<UserSession> UserSession { get; set; }
    public DbSet<ApplicationRole> RolesExtended { get; set; }
    
    // Applicant Tables
    public DbSet<Experience> Experience { get; set; }
    public DbSet<Qualification> Qualification { get; set; }
    public DbSet<ProfileSkill> ApplicantSkill { get; set; }
    public DbSet<ProfileLanguage> LanguageProficiency { get; set; }
    public DbSet<ResidenceAddress> ResidenceAddress { get; set; }
    public DbSet<TrainingCourse> TrainingCourse { get; set; }
    public DbSet<Achievement> Achievement { get; set; }
    public DbSet<ProfileAdditionalAttachment> AdditionalAttachmentApplicant { get; set; }

    public DbSet<ProfileChangeRequest> ProfileChangeRequests { get; set; }
    public DbSet<ReviewItem> ReviewItem { get; set; }
    public DbSet<ProfileAssignment> ProfileAssignment { get; set; }
    public DbSet<ProfileReviewDecision> ProfileReviewDecision { get; set; }
    public DbSet<AuditTrailEntry> AuditTrailEntry { get; set; }
    public DbSet<UserProfileLogger> UserProfileLogger { get; set; }
    
    // Recruitment Tables
    public DbSet<Job> Jobs { get; set; }
    public DbSet<Invitation> Invitations { get; set; }
    public DbSet<JobCondition> JobConditions { get; set; }
    public DbSet<JobDegree> JobDegrees { get; set; }
    public DbSet<JobPointConfiguration> JobPointConfigurations { get; set; }
    public DbSet<InvitationExpiryConfiguration> InvitationExpiryConfigurations { get; set; }
    public DbSet<JobCategoryCandidateSettings> JobCategoryCandidateSettings { get; set; }
    public DbSet<JobSkill> JobSkills { get; set; }
    public DbSet<JobResponsibility> JobResponsibilities { get; set; }
    public DbSet<JobRequiredAttachment> JobRequiredAttachments { get; set; }
    public DbSet<JobCandidateFilterSetting> JobCandidateFilterSettings { get; set; }
    public DbSet<JobCandidateTypePercentage> JobCandidateTypePercentages { get; set; }
    public DbSet<JobCandidateNationalityPercentage> JobCandidateNationalityPercentages { get; set; }
    public DbSet<JobCandidateFilterSpecialization> JobCandidateFilterSpecializations { get; set; }

    // Exams Tables
    public DbSet<Location> Locations { get; set; }
    public DbSet<Exam> Exams { get; set; }
    public DbSet<ExamPart> ExamParts { get; set; }
    public DbSet<ExamCategory> ExamCategories { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<TestSlot> TestSlots { get; set; }
    public DbSet<TestSlotStaff> TestSlotStaff { get; set; }
    public DbSet<TestSession> TestSessions { get; set; }
    public DbSet<TestSessionCandidate> TestSessionCandidates { get; set; }
    public DbSet<TestAttempt> TestAttempts { get; set; }
    public DbSet<TestAttemptPart> TestAttemptParts { get; set; }
    public DbSet<TestAttemptQuestion> TestAttemptQuestions { get; set; }
    public DbSet<TestAttemptInterruption> TestAttemptInterruptions { get; set; }
    public DbSet<ExamResultReport> ExamResultReports { get; set; }
    public DbSet<ExamResultCandidate> ExamResultCandidates { get; set; }
    public DbSet<ExamExemptionDecision> ExamExemptionDecisions { get; set; }
    public DbSet<HomeSuccessStory> HomeSuccessStories { get; set; }
    public DbSet<FAQ> Faqs { get; set; }
    // Notification Tables
    public DbSet<EmailQueue> EmailQueues { get; set; }
    public DbSet<EmailTemplate> EmailTemplates { get; set; }
    public DbSet<Notification> Notifications { get; set; }

    public DbSet<Resource> Resources { get; set; }
    public DbSet<MajorSkill> MajorSkill { get; set; }
    public DbSet<KawaderQid> KawaderQids { get; set; }

    // Minister Office Tables
    public DbSet<MinisterOfficeCandidate> MinisterOfficeCandidates { get; set; }
    public DbSet<MinisterOfficeCandidateAuditLog> MinisterOfficeCandidateAuditLogs { get; set; }

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
        
        // Notification table specific indexes
        builder.Entity<Notification>()
            .HasIndex(n => n.IdempotencyKey)
            .IsUnique()
            .HasFilter("[IdempotencyKey] IS NOT NULL");
        
        builder.Entity<Notification>()
            .HasIndex(n => n.NextRetryAt);
        
        // Automatically add indexes to common query/filtering fields on all models
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            var clrType = entityType.ClrType;

            var props = entityType.GetProperties()
                .Where(p => !p.IsPrimaryKey() && !p.IsForeignKey());

            foreach (var prop in props)
            {
                var name = prop.Name;
                var underlyingType = Nullable.GetUnderlyingType(prop.ClrType) ?? prop.ClrType;
                
                if ((name.EndsWith("Id") && underlyingType == typeof(Guid)) ||
                    name == "NationalNumber" ||
                    name == "Status" ||
                    name == "Provider")
                {
                    builder.Entity(clrType).HasIndex(name);
                }
            }
        }

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
            
            // Create index for IsDeleted globally to enhance performance on the query filter
            builder.Entity(clrType).HasIndex(nameof(ISoftDelete.IsDeleted));

            if (!typeof(BaseEntity).IsAssignableFrom(clrType)) continue;
            if (clrType == typeof(User)) continue;
            var entity = builder.Entity(clrType);
            
            // Create indexes for CreatedDate and CreatedById globally on BaseEntity implementing classes to speed up sorting and user lookups
            entity.HasIndex(nameof(BaseEntity.CreatedDate));
            entity.HasIndex(nameof(BaseEntity.CreatedById));

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
        var now = DateTime.UtcNow;


        ProtectVerifiedIdentityFields();

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
    private void ProtectVerifiedIdentityFields()
    {
        if (identityFieldProtectionContext.AllowVerifiedIdentityWrite)
            return;

        foreach (var entry in ChangeTracker.Entries<User>())
        {
            if (entry.State != EntityState.Modified)
                continue;

            if (!IsTrustedIdentityUser(entry.Entity.Id))
                continue;

            ProtectProperty(entry, nameof(User.FullNameAr));
            ProtectProperty(entry, nameof(User.FullNameEn));
            ProtectProperty(entry, nameof(User.PhoneNumber));
        }

        foreach (var entry in ChangeTracker.Entries<UserProfile>())
        {
            if (entry.State != EntityState.Modified)
                continue;

            if (!IsLockedProvider(entry.Entity.Provider))
                continue;

            ProtectProperty(entry, nameof(Domain.Entities.Users.UserProfile.NationalNumber));
            ProtectProperty(entry, nameof(Domain.Entities.Users.UserProfile.QIDExpiry));
            ProtectProperty(entry, nameof(Domain.Entities.Users.UserProfile.BirthDate));
            ProtectProperty(entry, nameof(Domain.Entities.Users.UserProfile.NationalityId));
            ProtectProperty(entry, nameof(Domain.Entities.Users.UserProfile.GenderId));
            var candidateTypeProperty = entry.Property(nameof(Domain.Entities.Users.UserProfile.CandidateTypeId));
            if (candidateTypeProperty.OriginalValue is Guid originalId &&
                CandidateTypeIds.IsVerifiedIdentityLocked(originalId))
            {
                ProtectProperty(entry, nameof(Domain.Entities.Users.UserProfile.CandidateTypeId));
            }
        }
    }

    private static bool IsLockedProvider(string? provider)
    {
        if (string.IsNullOrWhiteSpace(provider))
            return false;

        return provider.Equals("qatarpass", StringComparison.OrdinalIgnoreCase) ||
               provider.Equals("qatarresidentotp", StringComparison.OrdinalIgnoreCase);
    }

    private bool IsTrustedIdentityUser(Guid userId)
    {
        var trackedProfile = ChangeTracker.Entries<UserProfile>()
            .Select(x => x.Entity)
            .FirstOrDefault(p => p.UserId == userId);

        if (trackedProfile is not null)
            return IsLockedProvider(trackedProfile.Provider);

        return UserProfile.AsNoTracking()
            .Any(p => p.UserId == userId &&
                      (p.Provider == "qatarpass" || p.Provider == "qatarresidentotp"));
    }

    private void ProtectProperty<TEntity>(EntityEntry<TEntity> entry, string propertyName)
        where TEntity : class
    {
        var property = entry.Property(propertyName);
        if (!property.IsModified)
            return;

        if (CanBootstrapVerifiedField(property.OriginalValue, property.CurrentValue))
            return;

        var originalValue = property.OriginalValue?.ToString() ?? "null";
        var currentValue = property.CurrentValue?.ToString() ?? "null";

        logger.Warning(
            "Blocked write to verified identity field. Entity={Entity} Property={Property} Original={Original} Current={Current}",
            typeof(TEntity).Name,
            propertyName,
            originalValue,
            currentValue);

        property.CurrentValue = property.OriginalValue;
        property.IsModified = false;
    }

    private static bool CanBootstrapVerifiedField(object? originalValue, object? currentValue)
    {
        if (currentValue is null)
            return false;

        return originalValue switch
        {
            null => true,
            string text => string.IsNullOrWhiteSpace(text),
            _ => false
        };
    }

    private Guid? GetCurrentUserId()
    {
        try
        {
            var userIdClaim = this.GetService<IHttpContextAccessor>().HttpContext?.User
                .FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null ? Guid.Parse(userIdClaim.Value) : null;
        }
        catch (Exception)
        {
            return AdminUserIds.Admin1UserId;
        }
    }
    public override int SaveChanges()
    {
        return SaveChangesAsync().GetAwaiter().GetResult();
    }
}
