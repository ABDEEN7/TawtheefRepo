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
    public Guid UserTypeId { get; set; }
    public UserType? UserType { get; set; }
    
    public Guid? CreatedById { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public Guid? UpdatedById { get; set; }
    public DateTimeOffset? UpdatedDate { get; set; }
    public bool IsDeleted { get; set; }
    public Guid? DeletedById { get; set; }
    public DateTimeOffset? DeletedDate { get; set; }
    
    public ICollection<Notification.Notification> Notifications { get; init; } = [];
    
    private readonly List<RefreshToken> _refreshTokens = [];
    public IReadOnlyCollection<RefreshToken> RefreshTokens  => _refreshTokens.AsReadOnly();
    
    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string? CurrentAuthToken { get; set; }
    [MaxLength(length: 128)]
    public string? ActiveDeviceId { get; private set; }
    
    public string? OtpReference { get; private set; }
    public DateTime? OtpExpiry { get; private set; }
    public int OtpAttempts { get; set; }
    public int OtpSends { get; private set; }       // for resend throttle

    public void SetOtp(string otpReference, DateTime expiryUtc)
    {
        OtpReference = otpReference;
        OtpExpiry = expiryUtc;
        OtpAttempts = 0;
        OtpSends++;
    }

    public void ClearOtp()
    {
        OtpReference = null;
        OtpExpiry = null;
        OtpAttempts = 0;
        OtpSends = 0;
    }
    public void AddRefreshToken(string token, DateTimeOffset expires, string sid, string? ip = null, string? userDeviceId = null)
    {
        _refreshTokens.Add(new RefreshToken
        {
            Token = token,
            Expires = expires,
            CreatedDate = DateTimeOffset.UtcNow,
            CreatedByIp = ip,
            UserId = Id,
            UserDeviceId = userDeviceId,
            SecurityStamp = sid
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
        
        user.AddDomainEvent(new UserRegisteredEvent(user.Id, email, user.GivenNameEn, userTypeId, DateTime.Now));

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
