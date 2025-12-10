using System.Security.Claims;
using Mapster;
using Tawtheef.Application.Common.Constants;
using Tawtheef.Application.Features.Operations.Admin.Roles.DTOs;
using Tawtheef.Application.Features.Operations.Admin.Users.DTOs;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Operations.Admin.Users.Mappers;

public sealed class UserProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<User, UserListItemDto>()
            .Map(dest => dest.Name, src => string.IsNullOrWhiteSpace(src.FullNameAr) ? src.FullNameEn : src.FullNameAr)
            .Map(dest => dest.Email, src => src.Email ?? string.Empty)
            .Map(dest => dest.Roles, _ => Array.Empty<string>());

        config.NewConfig<User, UserRoleAssignmentDto>()
            .Map(dest => dest.Name, src => string.IsNullOrWhiteSpace(src.FullNameAr) ? src.FullNameEn : src.FullNameAr)
            .Map(dest => dest.Email, src => src.Email ?? string.Empty)
            .Map(dest => dest.Roles, _ => new List<RoleSummaryDto>())
            .Map(dest => dest.AssignedRoleIds, _ => Array.Empty<string>());

        config.NewConfig<RoleWithClaims, RoleSummaryDto>()
            .Map(dest => dest.Id, src => src.Role.Id)
            .Map(dest => dest.NameAr, src => GetNameClaim(src.Claims, RoleClaimTypes.NameArabic, src.Role.Name))
            .Map(dest => dest.NameEn, src => GetNameClaim(src.Claims, RoleClaimTypes.NameEnglish, src.Role.Name));
    }

    private static string GetNameClaim(IEnumerable<Claim> claims, string type, string? fallback)
    {
        return claims.FirstOrDefault(c => c.Type == type)?.Value ?? fallback ?? string.Empty;
    }
}
