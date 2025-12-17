using System.Linq;
using Mapster;
using Tawtheef.Application.Features.Operations.Admin.Offices.DTOs;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Operations.Admin.Offices.Mappers;

public sealed class OfficeProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Country, CountryLookupDto>();

        config.NewConfig<OfficeSupportedCountry, OfficeSupportedCountryDto>();

        config.NewConfig<Office, OfficeDto>()
            .Map(dest => dest.CountryNameAr, src => src.Country != null ? src.Country.NameAr : string.Empty)
            .Map(dest => dest.CountryNameEn, src => src.Country != null ? src.Country.NameEn : string.Empty)
            // .Map(dest => dest.AdminEmail,
            //     src => src.OfficeUsers != null
            //         ? src.OfficeUsers.FirstOrDefault(u => u.UserType)?.Email ?? string.Empty
            //         : string.Empty)
            .Map(dest => dest.SupportedCountries,
                src => src.SupportedCountries
                    .Where(sc => !sc.IsDeleted)
                    .OrderBy(sc => sc.DisplayOrder));
    }
}
