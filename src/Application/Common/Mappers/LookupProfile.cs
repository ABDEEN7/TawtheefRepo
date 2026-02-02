using Mapster;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Common.Models;
using Tawtheef.Domain.Common;

namespace Tawtheef.Application.Common.Mappers;

public class LookupProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<LookupBase, string>()
            .MapWith(src =>
                MapContext.Current!
                    .GetService<ILocalizationService>()!
                    .GetLocalizedName(src));
       
       config.NewConfig<LookupBase, DropdownOptions>()
           .Map(dest => dest.Id, src => src.Id)
           .Map(dest => dest.BackendName, src => src.BackendName)
           .AfterMapping((src, dest) =>
           {
               var localized = MapContext.Current!.GetService<ILocalizationService>();
               dest.Name = localized.GetLocalizedName(src);
               dest.Description = localized.GetLocalizedDescription(src);
               dest.AdditionalData = new { src.NameAr, src.NameEn };
           });
       
        config.NewConfig<LocalizedLookupBase, string>()
            .MapWith(src =>
                MapContext.Current!
                    .GetService<ILocalizationService>()!
                    .GetLocalizedName(src));
       
       config.NewConfig<LocalizedLookupBase, DropdownOptions>()
           .Map(dest => dest.Id, src => src.Id)
           .AfterMapping((src, dest) =>
           {
               var localized = MapContext.Current!.GetService<ILocalizationService>();
               dest.Name = localized.GetLocalizedName(src);
               dest.Description = localized.GetLocalizedDescription(src);
               dest.AdditionalData = new { src.NameAr, src.NameEn };
           });
    }
}
