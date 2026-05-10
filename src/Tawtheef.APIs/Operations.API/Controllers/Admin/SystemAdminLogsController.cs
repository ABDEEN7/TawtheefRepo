using System.Text.Json;
using Application.Operation.Features.Admin.SystemAdminLogs.Queries;
using Application.Operation.Features.Admin.Users.Queries;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Tawtheef.Application.Common.Interfaces.Services.Security;
using Tawtheef.Application.Common.Security;
using Tawtheef.Domain.Entities.Logger;
using Tawtheef.Infrastructure.Data;
using Tawtheef.Infrastructure.Extensions;

namespace Operations.API.Controllers.Admin;

[ApiController]
[Route("api/system-admin-logs")]
[Microsoft.AspNetCore.Authorization.Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class SystemAdminLogsController(
    IMediator mediator,
    TawtheefDbContext dbContext,
    ICurrentUserService currentUserService) : ControllerBase
{
    [HttpGet]
    [AuthorizePermission(PermissionKeys.ProfileLogs.View)]
    public async Task<IActionResult> Get([FromQuery] GetSystemAdminLogsQuery query)
    {
        var result = await mediator.Send(query);
        return result.ToActionResult();
    }

    [HttpGet("lookups")]
    [AuthorizePermission(PermissionKeys.ProfileLogs.View)]
    public async Task<IActionResult> GetUsersLookup()
    {
        var result = await mediator.Send(new GetUsersLookupQuery());
        return result.ToActionResult();
    }

    [HttpPost("navigation")]
    public async Task<IActionResult> LogNavigation([FromBody] NavigationLogRequest request, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(currentUserService.UserId, out var userId))
            return NoContent();

        var notes = JsonSerializer.Serialize(new
        {
            eventType = "SidebarNavigation",
            actionKind = "Navigation",
            section = "Navigation",
            menuKey = request.MenuKey,
            menuLabel = request.MenuLabel,
            targetUrl = request.TargetUrl,
            previousUrl = request.PreviousUrl,
            message = "Sidebar navigation clicked"
        });

        dbContext.Set<ActionLog>().Add(new ActionLog
        {
            UserProfileId = Guid.Empty,
            UserId = userId,
            LogType = ActionLogType.Employee,
            ActionType = "Navigation.SidebarClick",
            Section = "Navigation",
            Notes = notes,
            CreatedDate = DateTime.UtcNow
        });

        await dbContext.SaveChangesAsync(cancellationToken);
        return NoContent();
    }
}

public sealed record NavigationLogRequest(
    string? MenuKey,
    string? MenuLabel,
    string? TargetUrl,
    string? PreviousUrl);
