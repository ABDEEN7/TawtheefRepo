using System.Security.Claims;

namespace Tawtheef.Application.Features.Authenticator.DTOs;

internal sealed record ProviderProfileClaims(
    string? Email,
    string? FullName,
    string? Picture,
    string? Profile,
    string? Locale)
{
    public static ProviderProfileClaims FromAzure(ClaimsPrincipal p)
    {
        var email =
            p.FindFirstValue(ClaimTypes.Email) ??
            p.FindFirst("preferred_username")?.Value;

        var given =
            p.FindFirstValue(ClaimTypes.GivenName) ??
            p.FindFirst("given_name")?.Value;

        var surname =
            p.FindFirstValue(ClaimTypes.Surname) ??
            p.FindFirst("family_name")?.Value;

        var full =
            p.FindFirstValue(ClaimTypes.Name) ??
            p.FindFirst("name")?.Value ??
            $"{given} {surname}".Trim();

        return new ProviderProfileClaims(
            Email: email,
            FullName: string.IsNullOrWhiteSpace(full) ? null : full,
            Picture: p.FindFirst("picture")?.Value,
            Profile: p.FindFirst("profile")?.Value,
            Locale: p.FindFirst("locale")?.Value
        );
    }
}
