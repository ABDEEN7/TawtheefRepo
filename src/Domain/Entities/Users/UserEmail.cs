using System;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Users;

public class UserEmail: EventEntity
{
    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public required string Email { get; init; }
    public bool IsVerified { get; init; }
    /// <summary>
    /// "UserProvided","google","AzureAD"...
    /// </summary>
    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public required string Source { get; init; }
    public bool IsPrimary { get; init; }

    public Guid UserId { get; init; }
    public User? User { get; init; }
}
