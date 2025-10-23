using System;
using System.Collections.Generic;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Users;

public class UserSocialAccount: EventEntity
{
    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public required string ProviderUserId { get; init; }
    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string? Username { get; init; }
    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string? DisplayName { get; init; }
    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string? Email { get; init; }
    public bool? IsEmailVerified { get; init; }
    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string? PictureUrl { get; init; }
    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string? ProfileUrl { get; init; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string? ScopesJson { get; init; }
    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string? ClaimsJson { get; init; } 

    public DateTimeOffset LinkedAt { get; init; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? LastLoginAt { get; init; }
    public DateTimeOffset? LastSyncAt { get; init; }
    public SocialAccountStatus Status { get; init; } = SocialAccountStatus.Active;

    public Guid UserId { get; init; }
    public User? User { get; init; }
    
    public Guid ProviderId { get; init; }
    public SocialProvider? Provider { get; init; }
    
    public ICollection<UserSocialToken> Tokens { get; init; } = new List<UserSocialToken>();
}
