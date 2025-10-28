using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Identity;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Common.Interfaces;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Auth;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Events.User;
using Tawtheef.Domain.ValueObjects.User;

namespace Tawtheef.Domain.Entities.Users;

public class User : IdentityUser<Guid>, IBaseEntity, IHasDomainEvents
{
    [Required, StringLength(50)]
    public required string GivenNameEn { get; set; }
    [Required, StringLength(50)]
    public required string FamilyNameEn { get; set; }
    [StringLength(50)]
    public string? GivenNameAr  { get; set; }
    [StringLength(50)]
    public string? FamilyNameAr { get; set; }

    [NotMapped] public string FullNameEn => $"{this.GivenNameEn} {this.FamilyNameEn}".Trim();
    [NotMapped] public string FullNameAr => $"{this.GivenNameAr} {this.FamilyNameAr}".Trim();
    public DateTime? LastLoginDate { get; set; }
    
    [StringLength(2048)]
    public string? Avatar { get; set; }
    public string? PreferredUiLang { get; set; }
    public string? ProviderSource    { get; set; }
    public Guid UserTypeId { get; set; }
    public UserType? UserType { get; set; }
    
    public Guid? CreatedById { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public Guid? UpdatedById { get; set; }
    public DateTimeOffset? UpdatedDate { get; set; }
    public bool IsDeleted { get; set; }
    public Guid? DeletedById { get; set; }
    public DateTimeOffset? DeletedDate { get; set; }
    
    public ICollection<Notification.Notification>? Notifications { get; init; }
    
    private readonly List<RefreshToken> _refreshTokens = [];
    public IReadOnlyCollection<RefreshToken> RefreshTokens  => _refreshTokens.AsReadOnly();
    public DateTime? OtpExpiry { get; set; }
    [MaxLength(length: 6)]
    public string? OtpCode { get; set; }
    public int OtpAttempts { get; set; }
    
    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string? CurrentAuthToken { get; set; }
    public Guid? CurrentSessionId { get; private set; }
    [MaxLength(length: 128)]
    public string? ActiveDeviceId { get; private set; }
    private readonly List<BaseEvent> _domainEvents = [];

    [NotMapped]
    public IReadOnlyCollection<BaseEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void AddDomainEvent(BaseEvent e) => _domainEvents.Add(e);
    public void RemoveDomainEvent(BaseEvent e) => _domainEvents.Remove(e);
    public void ClearDomainEvents() => _domainEvents.Clear();

    public void StartNewExclusiveSession(string deviceId, Guid sessionId)
    {
        CurrentSessionId = sessionId;
        ActiveDeviceId = deviceId;
    }
    public void AddRefreshToken(string token, DateTime expires, string? ip = null, string? userDeviceId = null)
    {
        _refreshTokens.Add(new RefreshToken
        {
            Token = token,
            Expires = expires,
            CreatedDate = DateTime.UtcNow,
            CreatedByIp = ip,
            UserId = Id,
            UserDeviceId = userDeviceId
        });
    }
    public void RemoveOldRefreshTokens(int keepCount)
    {
        var old = _refreshTokens
            .Where(rt => !rt.IsActive)
            .OrderByDescending(rt => rt.CreatedDate)
            .Skip(keepCount)
            .ToList();

        foreach (var o in old) _refreshTokens.Remove(o);
    }
    public void UpdateLoginInfo(string authToken, DateTime loginDate)
    {
        CurrentAuthToken = authToken;
        LastLoginDate = loginDate;
    }
    public static Result<User> Register(
        string email,
        string displayName,
        string userTypeRaw
    )
    {
        var name = ValueObjects.User.FullName.TryParse(displayName);
        if (name.IsFailure) return name.ConvertFailure<User>();

        var userType = UserTypeParser.TryFrom(userTypeRaw);
        if (userType.IsFailure) return userType.ConvertFailure<User>();

        var userResult = userType.Value == UserTypeIds.Applicant
            ? ApplicantUser.Register(email, displayName)
            : EmployeeUser.Register(email, displayName);

        if (userResult.IsFailure) return userResult;
        
        var user = userResult.Value;
        user.UserTypeId = userType.Value;
        
        user.AddDomainEvent(new UserRegisteredEvent(user.Id, email, user.GivenNameEn, userType.Value, DateTime.Now));

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
