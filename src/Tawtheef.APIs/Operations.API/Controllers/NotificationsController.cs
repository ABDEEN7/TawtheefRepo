using System.Security.Claims;
using Cortex.Mediator;
using FluentResults;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tawtheef.Application.Features.Notifications.Queries;
using Tawtheef.Domain.Constants;
using Tawtheef.Infrastructure.Extensions;

namespace Operations.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class NotificationsController(IMediator mediator) : ControllerBase
{
    private Result<Guid> UserId => User.FindFirst(ClaimTypes.NameIdentifier)?.Value switch
    {
        null => Result.Fail<Guid>(ErrorsCodes.InvalidUserIdentifier),
        var id => Result.Ok(Guid.Parse(id))
    };

    [HttpGet]
    public async Task<IActionResult> GetUserNotifications([FromQuery] int limit = 10)
    {
        if (UserId.IsFailed)
        {
            return Unauthorized(UserId.Errors);
        }

        var clampedLimit = Math.Clamp(limit, 1, 50);
        var result = await mediator.Send(new GetUserNotificationsQuery(UserId.Value, clampedLimit));
        return result.ToActionResult();
    }
}
