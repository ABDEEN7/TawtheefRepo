using Microsoft.AspNetCore.Http;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Domain.Common.Interfaces;

namespace Tawtheef.Infrastructure.Services.Localization;

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
    public string GetLocalizedFullName(ILocalizedFullName? source)
    {
        if (source == null) return string.Empty;
        return source.GetLocalizedName(GetCurrentLanguage());
    }
    public string? GetLocalizedDescription(ILocalizedDescription? source)
    {
        if (source == null) return string.Empty;
        return source.GetLocalizedDescription(GetCurrentLanguage());
    }
    
    public string GetLocalizedValue(string valueAr, string valueEn)
    {
        return string.Equals(GetCurrentLanguage(), "ar", StringComparison.OrdinalIgnoreCase)
            ? valueAr
            : valueEn;
    }
}
