using System.Security.Claims;
using MediatR;
using FluentResults;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tawtheef.Application.Features.Notifications.Commands;
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
    public async Task<IActionResult> GetUserNotifications([FromQuery] GetUserNotificationsQuery query)
    {
        if (UserId.IsFailed) return Unauthorized(UserId.Errors);

        var clampedLimit = Math.Clamp(query.Limit, 1, 50);
        var result = await mediator.Send(query with {UserId = UserId.Value, Limit = clampedLimit});
        return result.ToActionResult();
    }

    [HttpGet("unread-count")]
    public async Task<IActionResult> GetUnreadCount()
    {
        if (UserId.IsFailed) return Unauthorized(UserId.Errors);
        
        var result = await mediator.Send(new GetUnreadNotificationCountQuery(UserId.Value));
        return result.ToActionResult();
    }

    [HttpPut("{id:guid}/read")]
    public async Task<IActionResult> MarkAsRead(Guid id)
    {
        if (UserId.IsFailed) return Unauthorized(UserId.Errors);
        
        var result = await mediator.Send(new MarkNotificationAsReadCommand(UserId.Value, id));
        return result.ToActionResult();
    }

    [HttpPut("read")]
    public async Task<IActionResult> MarkManyAsRead([FromBody] List<Guid>? notificationIds = null)
    {
        if (UserId.IsFailed) return Unauthorized(UserId.Errors);
        
        IResult<Unit> result;
        if (notificationIds is {Count: > 0})
        {
            result = await mediator.Send(new MarkManyNotificationsAsReadCommand(UserId.Value, notificationIds));
        }
        else
        {
            result = await mediator.Send(new MarkAllNotificationsAsReadCommand(UserId.Value));
        }
        
        return result.ToActionResult();
    }

    [HttpPut("{id:guid}/dismiss")]
    public async Task<IActionResult> Dismiss(Guid id)
    {
        if (UserId.IsFailed) return Unauthorized(UserId.Errors);
        var result = await mediator.Send(new DismissNotificationCommand(UserId.Value, id));
        return result.ToActionResult();
    }

    [HttpPut("dismiss")]
    public async Task<IActionResult> DismissMany([FromBody] List<Guid>? notificationIds = null)
    {
        if (UserId.IsFailed) return Unauthorized(UserId.Errors);
        IResult<Unit> result = notificationIds is {Count: > 0}
            ? await mediator.Send(new DismissManyNotificationsCommand(UserId.Value, notificationIds))
            : await mediator.Send(new DismissAllNotificationsCommand(UserId.Value));
        return result.ToActionResult();
    }
}

