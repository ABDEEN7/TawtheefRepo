using Mapster;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Common.Mappers;
using Tawtheef.Application.Features.Operations.Admin.Universities.DTOs;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Tawtheef.Application.Features.Operations.Admin.Universities.Mappers;

public sealed class UniversityProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<University, UniversityAdminDto>()
            .Map(dest => dest.CountryId, src => src.City != null ? src.City.CountryId : Guid.Empty)
            .Map(dest => dest.LogoAr, src => ResourceMapper.Resolve(src.LogoAr == null ? null : src.LogoAr.Url))
            .Map(dest => dest.LogoEn, src => ResourceMapper.Resolve(src.LogoEn == null ? null : src.LogoEn.Url))
            .Map(dest => dest.CountryName, src => src.City!.Country)
            .Map(dest => dest.CityName, src => src.City);
    }
}
