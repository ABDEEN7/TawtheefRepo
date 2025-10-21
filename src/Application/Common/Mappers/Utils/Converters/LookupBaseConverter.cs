using AutoMapper;
using Microsoft.AspNetCore.Http;
using Tawtheef.Application.Extensions;
using Tawtheef.Domain.Common;

namespace Tawtheef.Application.Common.Mappers.Utils.Converters;

public class LookupBaseConverter(IHttpContextAccessor httpContextAccessor) : ITypeConverter<LookupBase?,string?>
{
    public string? Convert(LookupBase? source, string? destination, ResolutionContext context)
    {
        if (source is null || string.IsNullOrEmpty(source.BackendName)) return string.Empty;
        var language = httpContextAccessor.HttpContext?.Items["Language"]?.ToString()?.ToLower() ?? "en";
        return source.GetLocalizedName(language);
    }
}
