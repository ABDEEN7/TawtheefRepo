using System;
using System.ComponentModel.DataAnnotations;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Users;

public static class SocialProviderIds
{
    public static readonly Guid Google = Guid.Parse("E8D3B8E0-60B5-41AE-B76E-5A5B5D9F3A1D");
    public static readonly Guid AzureAD = Guid.Parse("A3D5F2C1-48E7-4B2D-B7D1-7C8E9F0A1B2C");
}
public enum SocialAccountStatus { Active = 1, Revoked = 2, Unlinked = 3 }
public class SocialProvider : EventEntity
{
    /// <summary>
    /// "google","AzureAD"
    /// </summary>
    [Required, StringLength(200)]
    public required string Code { get; init; }
    [Required, StringLength(200)]
    public required string DisplayName { get; init; }
    public bool IsEnabled { get; init; } = true;
}
