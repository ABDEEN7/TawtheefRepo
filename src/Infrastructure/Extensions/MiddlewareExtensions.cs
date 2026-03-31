using Microsoft.AspNetCore.Builder;
using Tawtheef.Infrastructure.Middlewares;

namespace Tawtheef.Infrastructure.Extensions;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseLanguageMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<LanguageMiddleware>();
    }
}
