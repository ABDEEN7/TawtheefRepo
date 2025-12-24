using System.Linq;
using Mapster;
using Tawtheef.Application.Common.Models;
using Tawtheef.Application.Features.Operations.Admin.Offices.DTOs;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Operations.Admin.Offices.Mappers;

public sealed class OfficeProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Office, OfficeDto>()
            .Map(dest => dest.AdminEmail, src => src.OfficeAdmin == null ? null : src.OfficeAdmin.Email ?? null)
            .Map(dest => dest.SupportedCountries,src => src.SupportedCountries.Select(sc=>sc.Country)
                .OrderByDescending(sc => sc.CreatedDate));

        config.NewConfig<Office, OfficeDetailsDto>()
            .Map(dest => dest.Users, src => src.OfficeUsers)
            .Map(dest => dest.AdminEmail, src => src.OfficeAdmin == null ? null : src.OfficeAdmin.Email ?? null)
            .Map(dest => dest.SupportedCountries,src => src.SupportedCountries.Select(sc=>sc.Country)
                    .OrderByDescending(sc => sc.CreatedDate));
        
        //Create config for OfficeUserDto mapping if needed
        config.NewConfig<OfficeUser, OfficeUserDto>();
    }
}
