using Tawtheef.Domain.Common.Interfaces;

namespace Tawtheef.Application.Common.Interfaces.Services;

public interface ILocalizationService
{
    string GetLocalizedName(ILocalizedName? source);
    string GetLocalizedDescription(ILocalizedDescription? source);
}
