using Application.Operation.Features.Admin.Offices.DTOs;
using Mapster;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Admin.Offices.Mappers;

public sealed class OfficeProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Office, OfficeDto>()
            .Map(dest => dest.AdminEmail, src => src.OfficeAdmin == null ? null : src.OfficeAdmin.Email ?? null)
            .Map(dest => dest.AdminNameAr, src => src.OfficeAdmin == null ? string.Empty : src.OfficeAdmin.FullNameAr)
            .Map(dest => dest.AdminNameEn, src => src.OfficeAdmin == null ? string.Empty : src.OfficeAdmin.FullNameEn)
            .Map(dest => dest.SupportedCountries,src => src.SupportedCountries.Select(sc=>sc.Country)
                .OrderByDescending(sc => sc.CreatedDate));

        config.NewConfig<Office, OfficeDetailsDto>()
            .Map(dest => dest.Users, src => (src.OfficeUsers ?? Enumerable.Empty<OfficeUser>())
                .Select(user => new OfficeUserDto
                {
                    Id = user.Id,
                    FullNameAr = user.FullNameAr,
                    FullNameEn = user.FullNameEn,
                    Email = user.Email ?? string.Empty,
                    IsBlocked = user.IsBlocked,
                    IsAdmin = user.Id == src.OfficeAdminId
                }))
            .Map(dest => dest.AdminEmail, src => src.OfficeAdmin == null ? null : src.OfficeAdmin.Email ?? null)
            .Map(dest => dest.AdminNameAr, src => src.OfficeAdmin == null ? string.Empty : src.OfficeAdmin.FullNameAr)
            .Map(dest => dest.AdminNameEn, src => src.OfficeAdmin == null ? string.Empty : src.OfficeAdmin.FullNameEn)
            .Map(dest => dest.SupportedCountries,src => src.SupportedCountries.Select(sc=>sc.Country)
                    .OrderByDescending(sc => sc.CreatedDate));
        
        //Create config for OfficeUserDto mapping if needed
        config.NewConfig<OfficeUser, OfficeUserDto>();
    }
}
