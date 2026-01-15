using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Tawtheef.Application.Common.Interfaces.Services.Security;

namespace Tawtheef.Infrastructure.Services.Identity;

public class CurrentUserService(IHttpContextAccessor ctx) : ICurrentUserService
{
    public string? UserId =>
        ctx.HttpContext?.User.FindFirst("sub")?.Value ??
        ctx.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
}