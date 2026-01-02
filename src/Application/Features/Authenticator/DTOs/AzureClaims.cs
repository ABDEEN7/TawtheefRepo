using System.Security.Claims;

namespace Tawtheef.Application.Features.Authenticator.DTOs;

internal sealed record AzureClaims(
    string ProviderKey,
    string Email,
    string? GivenName,
    string? Surname,
    string? FullName)
{
    public static AzureClaims From(ClaimsPrincipal p)
    {
        var email =
            p.FindFirst(ClaimTypes.Email)?.Value ??
            p.FindFirst("preferred_username")?.Value ??
            p.FindFirst("emails")?.Value ??
            string.Empty;

        var given =
            p.FindFirst(ClaimTypes.GivenName)?.Value ??
            p.FindFirst("given_name")?.Value;

        var surname =
            p.FindFirst(ClaimTypes.Surname)?.Value ??
            p.FindFirst("family_name")?.Value;

        var full =
            p.FindFirst(ClaimTypes.Name)?.Value ??
            p.FindFirst("name")?.Value ??
            $"{given} {surname}".Trim();

        return new AzureClaims(
            ProviderKey: GetProviderKey(p) ?? string.Empty,
            Email: email,
            GivenName: given,
            Surname: surname,
            FullName: string.IsNullOrWhiteSpace(full) ? null : full
        );
    }
    
    private static string? GetProviderKey(ClaimsPrincipal p)
    {
        var oid =
            p.FindFirst("oid")?.Value ??
            p.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value ??
            p.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var tid =
            p.FindFirst("tid")?.Value ??
            p.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid")?.Value;

        if (!string.IsNullOrWhiteSpace(oid) && !string.IsNullOrWhiteSpace(tid))
            return $"{tid}:{oid}";

        return oid ?? p.FindFirst("sub")?.Value;
    }

    public (string Given, string Surname) GetBestEffortNameParts()
    {
        if (!string.IsNullOrWhiteSpace(GivenName) && !string.IsNullOrWhiteSpace(Surname))
            return (GivenName!, Surname!);

        if (!string.IsNullOrWhiteSpace(FullName))
        {
            var parts = FullName!.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var given = parts.FirstOrDefault() ?? "User";
            var sur   = parts.Length > 1 ? string.Join(' ', parts.Skip(1)) : "Account";
            return (given, sur);
        }

        return ("User", "Account");
    }
}
