using Microsoft.AspNetCore.Http;

namespace Tawtheef.Application.Common.Interfaces.Services;

public interface IRequestBodyCapture
{
    Task<string?> TryGetRedactedBodyAsync(HttpContext ctx);
}
