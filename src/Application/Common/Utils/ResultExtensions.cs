using Microsoft.AspNetCore.Http;

namespace Tawtheef.Application.Common.Utils;

public sealed class HeaderResult(IResult inner, string name, string value) : IResult
{
    public async Task ExecuteAsync(HttpContext httpContext)
    {
        httpContext.Response.Headers[name] = value;
        await inner.ExecuteAsync(httpContext);
    }
}

public static class ResultExtensions
{
    public static IResult WithHeader(this IResult result, string name, string value)
        => new HeaderResult(result, name, value);
}
