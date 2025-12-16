using Mapster;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Features.Authenticator.DTOs.Responses;
using Tawtheef.Domain.Entities;

namespace Tawtheef.Application.Common.Mappers;

public sealed class ResourceMapper : IRegister
{
    public const string MediaKey = "media";

    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Resource, string>()
            .Map(dest => dest, src => Resolve(src.Url));
        
        config.NewConfig<Resource?, string>()
            .Map(dest => dest, src => src == null ? null : Resolve(src.Url));

        config.NewConfig<Resource, FileRefDto>()
            .Map(dest => dest.ResourceId, src => src.Id)
            .Map(dest => dest.FileName,   src => src.Name)
            .Map(dest => dest.Url,        src => Resolve(src.Url));

        config.NewConfig<Resource?, FileRefDto?>()
            .MapWith(src => src == null ? null : new FileRefDto
            {
                ResourceId = src.Id,
                FileName   = src.Name,
                Url        = Resolve(src.Url)
            });
    }

    private static string Resolve(string? url)
    {
        var ctx = MapContext.Current;
        if (ctx?.Parameters is null) return url ?? string.Empty;

        if (!ctx.Parameters.TryGetValue(MediaKey, out var obj) || obj is not IMediaUrlResolver media)
            return url ?? string.Empty;

        return media.ResolveAbsolute(url);
    }
}
