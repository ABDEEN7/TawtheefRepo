using System;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Users;

public class UserSocialToken: EventEntity
{
    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string? AccessTokenEnc { get; init; }
    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string? RefreshTokenEnc { get; init; }
    public DateTimeOffset? ExpiresAt { get; init; }
    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string? TokenType { get; init; }
    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string? Scope { get; init; }
    public DateTimeOffset? RevokedAt { get; init; }

    public Guid UserSocialAccountId { get; init; }
    public UserSocialAccount? UserSocialAccount { get; init; }
}
