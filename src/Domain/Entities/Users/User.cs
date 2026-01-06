using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Common.Interfaces;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Auth;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Events.User;
using Tawtheef.Domain.ValueObjects.User;

namespace Tawtheef.Domain.Entities.Users;

public class User : IdentityUser<Guid>, IBaseEntity, IHasDomainEvents, ILocalizedFullName
{
    [Required, StringLength(100)]
    public required string FullNameEn { get; set; }
    [Required, StringLength(100)]
    public required string FullNameAr { get; set; }
    public bool IsBlocked { get; set; }
    public DateTime? LastLoginDate { get; set; }
    
    [StringLength(2048)]
    public string? Avatar { get; set; }
    public Guid UserTypeId { get; set; }
    public UserType? UserType { get; init; }
    
    public Guid? CreatedById { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public Guid? UpdatedById { get; set; }
    public DateTimeOffset? UpdatedDate { get; set; }
    public bool IsDeleted { get; set; }
    public Guid? DeletedById { get; set; }
    public DateTimeOffset? DeletedDate { get; set; }
    
    public virtual ICollection<Notification.Notification> Notifications { get; init; } = [];
    public virtual ICollection<RefreshToken> RefreshTokens { get; init; } = [];
    public ICollection<UserProfileLogger> UserProfileLoggers { get; set; } = [];
    
    [MaxLength(450)]
    public string? CurrentAuthToken { get; set; }
    [MaxLength(6)]
    public string? OtpReference { get; private set; }
    public DateTime? OtpExpiry { get; private set; }
    public int OtpAttempts { get; set; }
    /// <summary>
    /// Number of OTPs sent to the user
    /// </summary>
    public int OtpSends { get; private set; }

    public void SetOtp(string otpReference, DateTime expiryUtc)
    {
        OtpReference = otpReference;
        OtpExpiry = expiryUtc;
        OtpAttempts = 0;
        OtpSends++;
    }

    public Result ValidateOtp(string otp, DateTime utcNow, int maxAttempts)
    {
        if (OtpExpiry is null || string.IsNullOrWhiteSpace(OtpReference))
            return Result.Fail(ErrorsCodes.InvalidCode);

        if (OtpExpiry <= utcNow)
            return Result.Fail(ErrorsCodes.VerificationCodeExpired);

        if (OtpAttempts >= maxAttempts)
            return Result.Fail(ErrorsCodes.TooManyAttempts);

        if (!string.Equals(OtpReference, otp, StringComparison.Ordinal))
        {
            OtpAttempts++;
            return Result.Fail(ErrorsCodes.InvalidCode);
        }

        OtpReference = null;
        OtpExpiry = null;
        OtpAttempts = 0;

        return Result.Ok();
    }

    public static Result<User> Register(string email, string displayName, Guid userTypeId)
    {
        var name = FullName.TryParse(displayName);
        if (name.IsFailed) return Result.Fail<User>(name.Errors);
        
        var userResult = Result.Fail<User>("User type not handled");
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
            user.CreatedDate = DateTimeOffset.UtcNow;

        user.UserTypeId = userTypeId;
        
        // raise domain event with UTC timestamp
        user.AddDomainEvent(new UserRegisteredEvent(user.Id, email, user.FullNameEn, userTypeId, DateTime.UtcNow));

        return Result.Ok(user);
    }
    public void Block() => IsBlocked = true;
    
    //--------------------------------------------
    
    private readonly List<BaseEvent> _domainEvents = [];

    [NotMapped]
    public IReadOnlyCollection<BaseEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void AddDomainEvent(BaseEvent e) => _domainEvents.Add(e);
    public void RemoveDomainEvent(BaseEvent e) => _domainEvents.Remove(e);
    public void ClearDomainEvents() => _domainEvents.Clear();
}
