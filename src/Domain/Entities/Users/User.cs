using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Identity;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Common.Interfaces;
using Tawtheef.Domain.Entities.Auth;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Events.User;
using Tawtheef.Domain.ValueObjects.User;

namespace Tawtheef.Domain.Entities.Users;

public class User : IdentityUser<Guid>, IBaseEntity, IHasDomainEvents
{
    [Required, StringLength(100)]
    public required string FullNameEn { get; set; }
    [Required, StringLength(100)]
    public required string FullNameAr { get; set; }
    public DateTime? LastLoginDate { get; set; }
    
    [StringLength(2048)]
    public string? Avatar { get; set; }
    public Guid UserTypeId { get; set; }
    public UserType? UserType { get; set; }
    
    public Guid? CreatedById { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public Guid? UpdatedById { get; set; }
    public DateTimeOffset? UpdatedDate { get; set; }
    public bool IsDeleted { get; set; }
    public Guid? DeletedById { get; set; }
    public DateTimeOffset? DeletedDate { get; set; }
    
    public virtual ICollection<Notification.Notification> Notifications { get; init; } = [];
    public virtual ICollection<RefreshToken> RefreshTokens { get; init; } = [];
    
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
    public static Result<User> Register(string email, string displayName, Guid userTypeId)
    {
        var name = FullName.TryParse(displayName);
        if (name.IsFailure) return name.ConvertFailure<User>();

        var userResult = userTypeId == UserTypeIds.Applicant
            ? ApplicantUser.Register(email, displayName)
            : EmployeeUser.Register(email, displayName);

        if (userResult.IsFailure) return userResult;
        
        var user = userResult.Value;
        user.UserTypeId = userTypeId;
        
        user.AddDomainEvent(new UserRegisteredEvent(user.Id, email, user.FullNameEn, userTypeId, DateTime.Now));

        return Result.Success(user);
    }
    
    //--------------------------------------------
    
    private readonly List<BaseEvent> _domainEvents = [];

    [NotMapped]
    public IReadOnlyCollection<BaseEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void AddDomainEvent(BaseEvent e) => _domainEvents.Add(e);
    public void RemoveDomainEvent(BaseEvent e) => _domainEvents.Remove(e);
    public void ClearDomainEvents() => _domainEvents.Clear();
}
