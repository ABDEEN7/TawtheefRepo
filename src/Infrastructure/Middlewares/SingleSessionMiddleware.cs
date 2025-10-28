using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Infrastructure.Middlewares;

public class SingleSessionMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext context, UserManager<User> userManager)
    {
        var path = context.Request.Path.Value?.ToLowerInvariant() ?? "";
        if (path.StartsWith("/api/auth/login") || path.StartsWith("/api/auth/refresh") || path.StartsWith("/api/website/"))
        {
            await next(context);
            return;
        }

        var userPrincipal = context.User;
        if (userPrincipal.Identity?.IsAuthenticated != true)
        {
            await next(context);
            return;
        }

        var userIdStr = userPrincipal.FindFirstValue(ClaimTypes.NameIdentifier) ??
                        userPrincipal.FindFirstValue(JwtRegisteredClaimNames.Sub);
        var sidClaim = userPrincipal.FindFirstValue(JwtRegisteredClaimNames.Sid);

        if (!Guid.TryParse(userIdStr, out var userId) || !Guid.TryParse(sidClaim, out var sidFromToken))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Invalid token claims.");
            return;
        }

        var currentSid = await userManager.Users
            .Where(u => u.Id == userId)
            .Select(u => u.CurrentSessionId)
            .FirstOrDefaultAsync(context.RequestAborted);

        if (currentSid is null || currentSid.Value != sidFromToken)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Session expired or signed in elsewhere.");
            return;
        }

        await next(context);
    }
}
