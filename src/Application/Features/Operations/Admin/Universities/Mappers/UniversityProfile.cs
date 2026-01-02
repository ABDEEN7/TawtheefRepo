using Mapster;
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
            // .AfterMapping((src, dest) =>
            // {
            //     var localized = MapContext.Current!.GetService<ILocalizationService>();
            //     dest.CountryName = localized.GetLocalizedName(src.City!.Country);
            //     dest.CityName = localized.GetLocalizedName(src.City);
            // })
            ;
    }
}
