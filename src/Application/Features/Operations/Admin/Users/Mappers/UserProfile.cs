using Mapster;
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
            .Map(dest => dest.Email, src => src.Email ?? string.Empty);

        config.NewConfig<User, UserRoleAssignmentDto>()
            .Map(dest => dest.UserId, src => src.Id)
            .Map(dest => dest.Name, src => string.IsNullOrWhiteSpace(src.FullNameAr) ? src.FullNameEn : src.FullNameAr)
            .Map(dest => dest.Email, src => src.Email ?? string.Empty)
            .Map(dest => dest.IsBlocked, src => src.IsBlocked)
            .Map(dest => dest.LastLoginDate, src => src.LastLoginDate)
            .Map(dest => dest.Roles, _ => new List<RoleSummaryDto>()).Map<IReadOnlyCollection<Guid>,>(dest => dest.AssignedRoleIds, _ => []);

        config.NewConfig<RoleWithClaims, RoleSummaryDto>()
            .Map(dest => dest.Id, src => src.Role.Id)
            .Map(dest => dest.NameAr, src => src.Role.NameAr ?? src.Role.Name ?? string.Empty)
            .Map(dest => dest.NameEn, src => src.Role.NameEn ?? src.Role.Name ?? string.Empty)
            .Map(dest => dest.SystemName, src => src.Role.Name ?? string.Empty)
            .Map(dest => dest.IsSystemRole, src => src.Role.IsSystemRole);
    }
}
