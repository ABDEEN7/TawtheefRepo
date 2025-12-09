using System.Security.Claims;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Tawtheef.Application.Common.Constants;
using Tawtheef.Application.Features.Operations.Admin.Roles.DTOs;

namespace Tawtheef.Application.Features.Operations.Admin.Roles.Mappers;

public sealed class RoleProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<RoleWithClaims, RoleDto>()
            .Map(dest => dest.Id, src => src.Role.Id)
            .Map(dest => dest.NameAr, src => GetNameClaim(src.Claims, RoleClaimTypes.NameArabic, src.Role.Name))
            .Map(dest => dest.NameEn, src => GetNameClaim(src.Claims, RoleClaimTypes.NameEnglish, src.Role.Name))
            .Map(dest => dest.Permissions, src => GetPermissions(src.Claims));
    }

    private static string GetNameClaim(IEnumerable<Claim> claims, string type, string? fallback)
    {
        return claims.FirstOrDefault(c => c.Type == type)?.Value ?? fallback ?? string.Empty;
    }

    private static string[] GetPermissions(IEnumerable<Claim> claims)
    {
        return claims
            .Where(c => c.Type == RoleClaimTypes.Permission)
            .Select(c => c.Value)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }
}
