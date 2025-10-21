using System.Linq;
using System.Reflection;
using AutoMapper;
using Tawtheef.Application.Common.Mappers.Utils.Converters;
using Tawtheef.Application.Common.Mappers.Utils.Resolvers;
using Tawtheef.Application.Common.Models;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Common.Interfaces;

namespace Tawtheef.Application.Common.Mappers;

public class LookupsProfile : Profile
{
    public LookupsProfile()
    {
        CreateMap<LookupBase, string?>().ConvertUsing<LookupBaseConverter>();
        
        var lookupTypes = Assembly.GetAssembly(typeof(LookupBase))?
            .GetTypes()
            .Where(t => t.IsSubclassOf(typeof(LookupBase)) && !t.IsAbstract) ?? [];

        foreach (var type in lookupTypes)
        {
            // Create mapping from each lookup type to DropdownOptions
            CreateMap(type, typeof(DropdownOptions))
                .ForMember(nameof(DropdownOptions.Id), opt => opt.MapFrom(nameof(LookupBase.Id)))
                .ForMember(nameof(DropdownOptions.BackendName), opt => opt.MapFrom(nameof(LookupBase.BackendName)))
                .ForMember(nameof(DropdownOptions.Name), opt => 
                    opt.MapFrom<LocalizedNameObjectResolver, ILocalizedName>(src=> (LookupBase)src))
                .ForMember(nameof(DropdownOptions.Description), opt => 
                    opt.MapFrom<LocalizedDescriptionObjectResolver, ILocalizedDescription>(src => (LookupBase)src));
        }
    }
    
}
