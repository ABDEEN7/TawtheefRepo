using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Tawtheef.Application.Common.Interfaces.Services.Security;

namespace Tawtheef.Infrastructure.Middlewares;

public class SingleSessionMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext ctx, ISessionService sessionService)
    {
        var principal = ctx.User;
        if (principal.Identity?.IsAuthenticated == true)
        {
            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            var sid = principal.FindFirst(JwtRegisteredClaimNames.Sid)?.Value;

            if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(sid) && Guid.TryParse(userId, out var parsedUserId))
            {
                var isActive = await sessionService.IsActiveAsync(parsedUserId, sid, ctx.RequestAborted);
                if (!isActive)
                {
                    ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await ctx.Response.WriteAsJsonAsync(new
                    {
                        error = "session_revoked",
                        message = "Your session is no longer valid. You may have signed in elsewhere."
                    });
                    return;
                }
            }
        }

        await next(ctx);
    }
}
