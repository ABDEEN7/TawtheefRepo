using System.Security.Claims;
using Application.Operation.Features.Admin.Roles.DTOs;
using Mapster;
using Tawtheef.Application.Common.Security;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Admin.Roles.Mappers;

public sealed class RoleProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<RoleWithClaims, RoleDto>()
            .Map(dest => dest.Id, src => src.Role.Id)
            .Map(dest => dest.NameAr, src => src.Role.NameAr ?? src.Role.Name ?? string.Empty)
            .Map(dest => dest.NameEn, src => src.Role.NameEn ?? src.Role.Name ?? string.Empty)
            .Map(dest => dest.DescriptionAr, src => src.Role.DescriptionAr)
            .Map(dest => dest.DescriptionEn, src => src.Role.DescriptionEn)
            .Map(dest => dest.SystemName, src => src.Role.Name ?? string.Empty)
            .Map(dest => dest.IsSystemRole, src => src.Role.IsSystemRole)
            .Map(dest => dest.Permissions, src => GetPermissions(src.Claims));

        config.NewConfig<ApplicationRole, RoleLookupDto>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.NameAr, src => src.NameAr ?? src.Name ?? string.Empty)
            .Map(dest => dest.NameEn, src => src.NameEn ?? src.Name ?? string.Empty)
            .Map(dest => dest.SystemName, src => src.Name ?? string.Empty)
            .Map(dest => dest.IsSystemRole, src => src.IsSystemRole);
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
