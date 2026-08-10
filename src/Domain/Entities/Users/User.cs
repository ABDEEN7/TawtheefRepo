using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Common.Interfaces;
using Tawtheef.Domain.Configurations.Rules;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Auth;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Events.Operation.Employee.Profile;
using Tawtheef.Domain.Events.User;
using Tawtheef.Domain.ValueObjects.User;

namespace Tawtheef.Domain.Entities.Users;

[Index(nameof(UserTypeId))]
[Index(nameof(CreatedById))]
[Index(nameof(UpdatedById))]
[Index(nameof(DeletedById))]
public class User : IdentityUser<Guid>, IBaseEntity, IHasDomainEvents, ILocalizedFullName
{
    [Required, StringLength(100)] public string FullNameEn { get; set; } = null!;
    [Required, StringLength(100)]
    public string FullNameAr { get; set; } = null!;
    public bool IsBlocked { get; set; }
    public bool AgreedToTerms { get; set; }
    public DateTime? LastLoginDate { get; set; }
    
    [MaxLength(10)]
    public string? PreferredLanguage { get; set; }
    
    [StringLength(2048)]
    public string? Avatar { get; set; }
    public Guid UserTypeId { get; set; }
    public UserType? UserType { get; init; }
    
    public Guid? CreatedById { get; set; }
    public DateTime CreatedDate { get; set; }
    public Guid? UpdatedById { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public bool IsDeleted { get; set; }
    public Guid? DeletedById { get; set; }
    public DateTime? DeletedDate { get; set; }
    
    public virtual ICollection<Notification.Notification> Notifications { get; init; } = [];
    public virtual ICollection<RefreshToken> RefreshTokens { get; init; } = [];
    public virtual ICollection<UserProfileLogger> UserProfileLoggers { get; init; } = [];
    public virtual ICollection<ProfileAssignment> ProfileAssignments { get; init; } = [];
    
    public virtual ICollection<IdentityUserRole<Guid>> UserRoles { get; set; } = [];
    
    [MaxLength(450)]
    public string? CurrentAuthToken { get; set; }
    [MaxLength(6)]
    public string? OtpReference { get; private set; }
    public DateTime? OtpExpiry { get; private set; }
    public int OtpAttempts { get; set; }
    public DateTime? OtpLockedUntilUtc { get; private set; }
    /// <summary>
    /// Number of OTPs sent to the user
    /// </summary>
    public DateTime? OtpSendWindowStartUtc { get; private set; }
    public int OtpSendsInWindow { get; private set; }
    
    public Result CanSendOtp(DateTime utcNow, int maxSends, TimeSpan window)
    {
        if (OtpSendWindowStartUtc is null || utcNow - OtpSendWindowStartUtc >= window)
        {
            OtpSendWindowStartUtc = utcNow;
            OtpSendsInWindow = 0;
        }

        if (OtpSendsInWindow >= maxSends)
            return Result.Fail(ErrorsCodes.SendOtpLimitReached);

        return Result.Ok();
    }

    public void MarkOtpSent()
    {
        OtpSendsInWindow++;
    }

    public Result<ProfileAssignment> CreateProfileAssignmentIfAllowed(UserProfile profile, int currentLoad, int assignedThisRound, int? perEmployeeLimit, bool publishNotification = true)
    {
        // Respect per-employee cap for this distribution run
        if (assignedThisRound >= perEmployeeLimit)
            return Result.Fail<ProfileAssignment>(ErrorsCodes.DistributionPerEmployeeLimitReached);

        // Ensure profile is assignable (caller may have already filtered, but guard here as domain rule)
        if (!ProfileDistributionRules.AssignableStatuses.Contains(profile.Status) &&
            profile.Status != UserProfileStatus.Approved)
            return Result.Fail<ProfileAssignment>(ErrorsCodes.ProfileNotAssignable);

        // Apply domain changes
        if (profile.Status != UserProfileStatus.Approved)
            profile.Status = UserProfileStatus.UnderReview;

        var assignment = ProfileAssignment.Assign(profile.Id, this.Id, publishNotification);

        // keep aggregate consistency in memory
        ProfileAssignments.Add(assignment);

        return Result.Ok(assignment);
    }

    public void SetOtpReference(string otpReference, DateTime expiryUtc)
    {
        OtpReference = otpReference;
        OtpExpiry = expiryUtc;
        OtpAttempts = 0;
    }
    public Result ValidateOtp(string otp, DateTime utcNow, int maxAttempts, TimeSpan lockDuration)
    {
        // If currently locked, reject but do NOT increment attempts
        if (OtpLockedUntilUtc is not null && OtpLockedUntilUtc > utcNow)
            return Result.Fail(ErrorsCodes.TooManyAttempts);

        // Lock expired → reset
        if (OtpLockedUntilUtc is not null && OtpLockedUntilUtc <= utcNow)
        {
            OtpLockedUntilUtc = null;
            OtpAttempts = 0;
        }

        if (OtpExpiry is null || string.IsNullOrWhiteSpace(OtpReference))
            return Result.Fail(ErrorsCodes.InvalidCode);

        if (OtpExpiry <= utcNow)
            return Result.Fail(ErrorsCodes.VerificationCodeExpired);

        if (!string.Equals(OtpReference, otp, StringComparison.Ordinal))
        {
            OtpAttempts++;

            if (OtpAttempts >= maxAttempts)
            {
                OtpLockedUntilUtc = utcNow.Add(lockDuration);
                OtpReference = null;
                OtpExpiry = null;
            }

            return Result.Fail(ErrorsCodes.InvalidCode);
        }

        // success
        OtpReference = null;
        OtpExpiry = null;
        OtpAttempts = 0;
        OtpLockedUntilUtc = null;

        return Result.Ok();
    }

    public static Result<User> Register(string email, string displayName, Guid userTypeId)
    {
        var name = FullName.TryParse(displayName);
        if (name.IsFailed) return Result.Fail<User>(name.Errors);
        
        Result<User> userResult;
        if (userTypeId == UserTypeIds.Applicant)
            userResult = ApplicantUser.Register(email, displayName);
        else if (userTypeId == UserTypeIds.Employee)
            userResult = EmployeeUser.Register(email, displayName);
        else if (userTypeId == UserTypeIds.OfficeUser)
            userResult = OfficeUser.Register(email, displayName);
        else
            return Result.Fail<User>("Invalid user type");

        if (userResult.IsFailed) return userResult;
        var user = userResult.Value;

        // Ensure aggregate identity and creation timestamp are set (in case specific Register didn't)
        if (user.Id == Guid.Empty)
            user.Id = Guid.NewGuid();

        if (user.CreatedDate == default)
            user.CreatedDate = DateTime.UtcNow;

        user.UserTypeId = userTypeId;
        
        // raise domain event with UTC timestamp
        user.AddDomainEvent(new UserRegisteredEvent(user.Id, email, user.FullNameEn, userTypeId, DateTime.UtcNow));

        return Result.Ok(user);
    }
    public void Block() => IsBlocked = true;
    public string GetPreferredLanguage(string? defaultValue = "ar") => PreferredLanguage ?? defaultValue ?? "ar";
    
    //--------------------------------------------
    
    private readonly List<BaseEvent> _domainEvents = [];

    [NotMapped]
    public IReadOnlyCollection<BaseEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void AddDomainEvent(BaseEvent e) => _domainEvents.Add(e);
    public void RemoveDomainEvent(BaseEvent e) => _domainEvents.Remove(e);
    public void ClearDomainEvents() => _domainEvents.Clear();
}
