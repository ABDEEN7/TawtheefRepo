using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Identity;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Common.Interfaces;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Events.User;
using Tawtheef.Domain.ValueObjects.User;
using Gender = Tawtheef.Domain.Entities.Lookups.Gender;

namespace Tawtheef.Domain.Entities.Users;

public class User : IdentityUser<Guid>, IBaseEntity, IHasDomainEvents
{
    [Required, StringLength(50)]
    public required string FirstName { get; set; }
    [Required, StringLength(50)]
    public required string LastName { get; set; }
    [NotMapped]
    public string FullName => $"{FirstName} {LastName}";
    [StringLength(2048)]
    public string? Avatar { get; set; }
    [StringLength(2048)]
    public string? Bio { get; set; }
    public DateTime? LastLoginDate { get; set; }
    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string? CurrentAuthToken { get; set; }
    
    public Guid GenderId { get; set; }
    public Gender? Gender { get; init; }
    
    public Guid UserTypeId { get; set; }
    public UserType? UserType { get; set; }
    
    public Guid? CreatedById { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public Guid? UpdatedById { get; set; }
    public DateTimeOffset? UpdatedDate { get; set; }
    public bool IsDeleted { get; set; }
    public Guid? DeletedById { get; set; }
    public DateTimeOffset? DeletedDate { get; set; }
    
    public ICollection<Notification>? Notifications { get; set; }
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    public DateTime? OtpExpiry { get; set; }
    [MaxLength(length: 6)]
    public string? OtpCode { get; set; }
    public int OtpAttempts { get; set; }
    
    public ICollection<UserSocialAccount>? SocialAccounts { get; init; }
    public ICollection<UserEmail>? Emails { get; init; }

    private readonly List<BaseEvent> _domainEvents = [];

    [NotMapped]
    public IReadOnlyCollection<BaseEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void AddDomainEvent(BaseEvent e) => _domainEvents.Add(e);
    public void RemoveDomainEvent(BaseEvent e) => _domainEvents.Remove(e);
    public void ClearDomainEvents() => _domainEvents.Clear();
    
        public void AddRefreshToken(string token, DateTime expires, string? createdByIp = null)
    {
        RefreshTokens.Add(new RefreshToken
        {
            Token = token,
            Expires = expires,
            CreatedDate = DateTime.UtcNow,
            CreatedByIp = createdByIp,
            UserId = Id
        });
    }
    public void RemoveOldRefreshTokens(int maxRefreshTokens)
    {
        // Remove old refresh tokens, keeping the most recent ones
        RefreshTokens = RefreshTokens
            .OrderByDescending(x => x.CreatedDate)
            .Take(maxRefreshTokens)
            .ToList();
    }
    public void UpdateLoginInfo(string authToken, DateTime loginDate)
    {
        CurrentAuthToken = authToken;
        LastLoginDate = loginDate;
    }
    public static Result<User> Register(
        string email,
        string displayName,
        string genderRaw,
        string userTypeRaw,
        Func<Guid, string> userTypeNameResolver
    )
    {
        var name = ValueObjects.User.FullName.TryParse(displayName);
        if (name.IsFailure) return name.ConvertFailure<User>();

        var gender = ValueObjects.User.Gender.TryFrom(genderRaw);
        if (gender.IsFailure) return gender.ConvertFailure<User>();

        var userType = UserTypeParser.TryFrom(userTypeRaw);
        if (userType.IsFailure) return userType.ConvertFailure<User>();

        var userResult = userType.Value == UserTypeIds.Applicant
            ? ApplicantUser.Register(email, displayName, genderRaw, userTypeRaw)
            : EmployeeUser.Register(email, displayName, genderRaw, userTypeRaw);

        if (userResult.IsFailure) return userResult;
        
        var user = userResult.Value;
        user.UserTypeId = userType.Value;
        
        var typeName = userTypeNameResolver(user.UserTypeId);
        user.AddDomainEvent(new UserRegisteredEvent(user.Id, email, user.FirstName, typeName, user.GenderId, DateTime.Now));

        return Result.Success(user);
    }
    
    public Result RequestPasswordReset(DateTime whenUtc)
    {
        if (!EmailConfirmed)
            return Result.Failure(ErrorsCodes.EmailNotVerified);

        AddDomainEvent(new UserPasswordResetRequestedEvent(
            UserId: Id,
            Email: Email ?? string.Empty,
            OccurredOn: whenUtc));

        return Result.Success();
    }
}
