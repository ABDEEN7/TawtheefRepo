using Mapster;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Domain.Entities;

namespace Tawtheef.Application.Common.Mappers;

public class ResourceProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.ForType<Resource, string>()
            .Map(dest => dest, src => 
                MapContext.Current!
                    .GetService<IMediaUrlResolver>()!
                    .ResolveAbsolute(src.Url));
    }
}
