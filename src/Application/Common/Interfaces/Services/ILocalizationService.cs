using Tawtheef.Domain.Common.Interfaces;

namespace Tawtheef.Application.Common.Interfaces.Services;

public interface ILocalizationService
{
    string GetCurrentLanguage();
    string GetLocalizedName(ILocalizedName? source);
    string GetLocalizedFullName(ILocalizedFullName? source);
    string? GetLocalizedDescription(ILocalizedDescription? source);
    string GetLocalizedValue(string valueAr, string valueEn);
    string GetLocalizedValue(string value);
}
