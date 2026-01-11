
using Cortex.Mediator;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Tawtheef.Application.Common;
using Tawtheef.Application.Common.Security;
using Tawtheef.Application.Features.Operations.Admin.Users.Commands;
using Tawtheef.Application.Features.Operations.Admin.Users.Queries;
using Tawtheef.Infrastructure.Extensions;

namespace Operations.API.Controllers.Admin;

[ApiController]
[Route("api/[controller]")]
[Microsoft.AspNetCore.Authorization.Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class UserManagementController(IMediator mediator) : ControllerBase
{
    [HttpGet("list-users")]
    [AuthorizePermission(PermissionKeys.Users.View)]
    public async Task<IActionResult> ListUsers([FromQuery] GetListUsersQuery query)
    {
        var result = await mediator.Send(query);
        return result.ToActionResult();
    }

    [HttpGet("{id:guid}/roles")]
    [AuthorizePermission(PermissionKeys.Users.View)]
    public async Task<IActionResult> GetUserRoles(Guid id)
    {
        var result = await mediator.Send(new GetUserRolesQuery(id));
        return result.ToActionResult();
    }

    [HttpGet("{id:guid}/role-ids")]
    [AuthorizePermission(PermissionKeys.Users.View)]
    public async Task<IActionResult> GetUserRoleIds(Guid id)
    {
        var result = await mediator.Send(new GetUserAssignedRoleIdsQuery(id));
        return result.ToActionResult();
    }

    [HttpPut("{id:guid}/roles")]
    [AuthorizePermission(PermissionKeys.Users.Manage)]
    public async Task<IActionResult> UpdateUserRoles(Guid id, [FromBody] UpdateUserRolesCommand command)
    {
        var result = await mediator.Send(command with { UserId = id });
        return result.ToActionResult();
    }

    [HttpPut("{id:guid}/block-status")]
    [AuthorizePermission(PermissionKeys.Users.Manage)]
    public async Task<IActionResult> UpdateUserBlockStatus(Guid id, [FromBody] UpdateUserBlockStatusCommand command)
    {
        var result = await mediator.Send(command with { UserId = id });
        return result.ToActionResult();
    }
}
