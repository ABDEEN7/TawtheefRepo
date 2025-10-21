using AutoMapper;
using Microsoft.AspNetCore.Http;
using Tawtheef.Domain.Common.Interfaces;

namespace Tawtheef.Application.Common.Mappers.Utils.Resolvers;

public class LocalizedNameResolver(IHttpContextAccessor httpContextAccessor)
    : IMemberValueResolver<object, object, ILocalizedName, string>
{
    public string Resolve(object source, object destination, ILocalizedName? sourceMember, 
        string? destMember, ResolutionContext context)
    {
        if (sourceMember is null) return string.Empty;

        var language = httpContextAccessor.HttpContext?.Items["Language"]?.ToString()?.ToLower() ?? "en";

        return sourceMember.GetLocalizedName(language);
    }
}
public class LocalizedNameObjectResolver(IHttpContextAccessor httpContextAccessor)
    : IMemberValueResolver<object, object, ILocalizedName, object?>
{
    public object? Resolve(object source, object destination, ILocalizedName? sourceMember, 
        object? destMember, ResolutionContext context)
    {
        if (sourceMember is null) return null;

        var language = httpContextAccessor.HttpContext?.Items["Language"]?.ToString()?.ToLower() ?? "en";
        var localized = sourceMember.GetLocalizedName(language);

        return localized;
    }
}
