using System.ComponentModel.DataAnnotations;
using Tawtheef.Domain.Common.Interfaces;

namespace Tawtheef.Application.Extensions;

// For classes that can't implement ILocalizedName but have the right properties
public static class LocalizedNameExtensions
{
    public static string? GetLocalizedName(this object? source,[AllowedValues("ar","en")] string language)
    {
        switch (source)
        {
            case null:
                return null;
            case ILocalizedName localized:
                return localized.GetLocalizedName(language);
            case ILocalizedFullName localized:
                return localized.GetLocalizedName(language);
            default:
            {
                // Reflection fallback (slower but flexible)
                var prop = source.GetType().GetProperty($"Name{language}");
                return prop?.GetValue(source) as string ?? source.GetType().GetProperty("NameEn")?.GetValue(source) as string;
            }
        }
    }
    public static string? GetLocalizedDescription(this object source,[AllowedValues("ar","en")] string language)
    {
        if (source is ILocalizedName localized)
            return localized.GetLocalizedName(language);
        
        // Reflection fallback (slower but flexible)
        var prop = source.GetType().GetProperty($"Description{language}");
        return prop?.GetValue(source) as string ?? source.GetType().GetProperty("DescriptionEn")?.GetValue(source) as string;
    }
}
