using Microsoft.AspNetCore.Http;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Domain.Common.Interfaces;

namespace Tawtheef.Infrastructure.Services;

public class LocalizationService(IHttpContextAccessor httpContextAccessor) : ILocalizationService
{
    private string GetCurrentLanguage()
    {
        return httpContextAccessor.HttpContext?.Items["Language"]?.ToString()?.ToLower() ?? "en";
    }

    public string GetLocalizedName(ILocalizedName? source)
    {
        if (source == null) return string.Empty;
        return source.GetLocalizedName(GetCurrentLanguage());
    }
    public string? GetLocalizedDescription(ILocalizedDescription? source)
    {
        if (source == null) return string.Empty;
        return source.GetLocalizedDescription(GetCurrentLanguage());
    }
}
