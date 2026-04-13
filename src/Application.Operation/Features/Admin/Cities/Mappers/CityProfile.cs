using Application.Operation.Features.Admin.Cities.DTOs;
using Mapster;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Application.Operation.Features.Admin.Cities.Mappers;

public class CityProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<City, CityAdminDto>()
            .Map(dest => dest.AdditionalData, src => new
            {
                NameEn = src.NameEn,
                NameAr = src.NameAr
            });
    }
}
