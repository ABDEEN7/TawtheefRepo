using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Infrastructure.Middlewares;

public class SingleSessionMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext ctx, UserManager<User> userManager)
    {
        var principal = ctx.User;
        if (principal.Identity?.IsAuthenticated == true)
        {
            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            var sid = principal.FindFirst(JwtRegisteredClaimNames.Sid)?.Value;

            if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(sid))
            {
                var user = await userManager.FindByIdAsync(userId);
                var currentStamp = user?.SecurityStamp;

                if (string.IsNullOrEmpty(currentStamp) || !string.Equals(currentStamp, sid, StringComparison.Ordinal))
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
