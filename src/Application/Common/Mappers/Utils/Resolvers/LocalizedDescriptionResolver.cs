using AutoMapper;
using Microsoft.AspNetCore.Http;
using Tawtheef.Application.Extensions;
using Tawtheef.Domain.Common.Interfaces;

namespace Tawtheef.Application.Common.Mappers.Utils.Resolvers;

public class LocalizedDescriptionResolver(IHttpContextAccessor httpContextAccessor)
    : IMemberValueResolver<object, object, ILocalizedDescription, string>
{
    public string Resolve(object source, object destination, ILocalizedDescription? sourceMember, 
        string? destMember, ResolutionContext context)
    {
        if (sourceMember is null) return string.Empty;

        var language = httpContextAccessor.HttpContext?.Items["Language"]?.ToString()?.ToLower() ?? "en";

        return sourceMember.GetLocalizedDescription(language);
    }
}
public class LocalizedDescriptionObjectResolver(IHttpContextAccessor httpContextAccessor)
    : IMemberValueResolver<object, object, ILocalizedDescription, object?>
{
    public object? Resolve(object source, object destination, ILocalizedDescription? sourceMember, 
        object? destMember, ResolutionContext context)
    {
        if (sourceMember is null) return null;

        var language = httpContextAccessor.HttpContext?.Items["Language"]?.ToString()?.ToLower() ?? "en";
        var localized = LocalizedNameExtensions.GetLocalizedDescription(sourceMember, language);

        return localized;
    }
}
