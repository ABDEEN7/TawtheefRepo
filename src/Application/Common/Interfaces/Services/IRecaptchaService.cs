using Tawtheef.Application.Common.Models;

namespace Tawtheef.Application.Common.Interfaces.Services;

public interface IRecaptchaService
{
    Task<RecaptchaResponse> VerifyAsync(string recaptchaToken, CancellationToken ct = default);
}
