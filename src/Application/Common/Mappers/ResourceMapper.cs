using Mapster;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Features.Authenticator.DTOs.Responses;
using Tawtheef.Domain.Entities;

namespace Tawtheef.Application.Common.Mappers;

public sealed class ResourceMapper : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Resource, string>()
            .Map(dest => dest,
                src => MapContext.Current!
                    .GetService<IMediaUrlResolver>()!
                    .ResolveAbsolute(src.Url));

        config.NewConfig<Resource, FileRefDto>()
            .Map(dest => dest.ResourceId, src => src.Id)
            .Map(dest => dest.FileName,   src => src.Name)
            .AfterMapping((src, dest) =>
            {
                var media = MapContext.Current!.GetService<IMediaUrlResolver>();
                dest.Url = media.ResolveAbsolute(src.Url);
            });
        
        config.NewConfig<Resource?, FileRefDto?>()
            .MapWith(src => src == null
                ? null
                : new FileRefDto
                {
                    ResourceId = src.Id,
                    FileName   = src.Name,
                    Url = MapContext.Current!
                        .GetService<IMediaUrlResolver>()!
                        .ResolveAbsolute(src.Url)
                });
    }
}
