using System.Security.Claims;
using MediatR;
using FluentResults;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tawtheef.Infrastructure.Extensions;
using Tawtheef.Application.Features.Notifications.Queries;
using Tawtheef.Domain.Constants;

namespace Recruitment.API.Controllers;

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
    public async Task<IActionResult> GetUserNotifications([FromQuery] bool unreadOnly = false, [FromQuery] int limit = 10, [FromQuery] DateTimeOffset? createdDateBefore = null)
    {
        if (UserId.IsFailed)
        {
            return Unauthorized(UserId.Errors);
        }

        var clampedLimit = Math.Clamp(limit, 1, 50);
        var result = await mediator.Send(new GetUserNotificationsQuery(UserId.Value, unreadOnly, clampedLimit, createdDateBefore));
        return result.ToActionResult();
    }

    [HttpGet("unread-count")]
    public async Task<IActionResult> GetUnreadCount()
    {
        if (UserId.IsFailed) return Unauthorized(UserId.Errors);
        var result = await mediator.Send(new GetUnreadNotificationCountQuery(UserId.Value));
        return result.ToActionResult();
    }

    [HttpPut("{id}/state")]
    public async Task<IActionResult> UpdateNotificationState(Guid id, [FromQuery] Tawtheef.Application.Features.Notifications.Commands.NotificationAction action)
    {
        if (UserId.IsFailed) return Unauthorized(UserId.Errors);
        var result = await mediator.Send(new Tawtheef.Application.Features.Notifications.Commands.UpdateNotificationStateCommand(UserId.Value, id, action));
        return result.ToActionResult();
    }

    [HttpPut("state")]
    public async Task<IActionResult> UpdateManyNotificationsState([FromQuery] Tawtheef.Application.Features.Notifications.Commands.NotificationAction action, [FromBody] List<Guid>? notificationIds = null)
    {
        if (UserId.IsFailed) return Unauthorized(UserId.Errors);
        
        IResult<Unit> result;
        if (notificationIds is {Count: > 0})
        {
            result = await mediator.Send(new Tawtheef.Application.Features.Notifications.Commands.UpdateManyNotificationsStateCommand(UserId.Value, notificationIds, action));
        }
        else
        {
            result = await mediator.Send(new Tawtheef.Application.Features.Notifications.Commands.UpdateAllNotificationsStateCommand(UserId.Value, action));
        }
        
        return result.ToActionResult();
    }
}

