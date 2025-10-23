using System;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Domain.Entities;

public class RefreshToken : EventEntity
{
    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public required string Token { get; init; }
    public DateTime Expires { get; init; }
    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string? CreatedByIp { get; init; }
    public DateTime? Revoked { get; set; }
    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string? RevokedByIp { get; set; }
    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string? ReplacedByToken { get; set; }
    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string? ReasonRevoked { get; set; }
    
    public bool IsExpired => DateTime.UtcNow >= Expires;
    public bool IsRevoked => Revoked != null;
    public bool IsActive => !IsRevoked && !IsExpired;
    
    public Guid? UserDeviceId { get; init; }
    public Guid UserId { get; init; }
    public User? User { get; init; }
}
