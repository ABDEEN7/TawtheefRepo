using Mapster;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Features.Operations.Admin.Universities.DTOs;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Tawtheef.Application.Features.Operations.Admin.Universities.Mappers;

public sealed class UniversityProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<University, UniversityAdminDto>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.BackendName, src => src.BackendName)
            .Map(dest => dest.Name, src => src.NameEn)
            .Map(dest => dest.NameAr, src => src.NameAr)
            .Map(dest => dest.NameEn, src => src.NameEn)
            .Map(dest => dest.DescriptionAr, src => src.DescriptionAr)
            .Map(dest => dest.DescriptionEn, src => src.DescriptionEn)
            .Map(dest => dest.CityId, src => src.CityId)
            .Map(dest => dest.CountryId, src => src.City != null ? src.City.CountryId : Guid.Empty)
            .Map(dest => dest.WebSite, src => src.WebSite)
            .Map(dest => dest.Phone, src => src.Phone)
            .Map(dest => dest.Email, src => src.Email)
            .Map(dest => dest.Code, src => src.Code)
            .Map(dest => dest.LogoAr, src => src.LogoAr)
            .Map(dest => dest.LogoEn, src => src.LogoEn)
            .Map(dest => dest.OriginalName, src => src.OriginalName)
            .Map(dest => dest.IsActive, src => src.IsActive)
            .AfterMapping((src, dest) =>
            {
                var localized = MapContext.Current!.GetService<ILocalizationService>();
                dest.CountryName = localized.GetLocalizedName(src.City!.Country);
                dest.CityName = localized.GetLocalizedName(src.City);
            });
    }
}
