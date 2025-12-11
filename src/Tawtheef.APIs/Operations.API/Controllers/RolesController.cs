using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tawtheef.Application.Features.Operations.Admin.Roles.Commands;
using Tawtheef.Application.Features.Operations.Admin.Roles.Queries;
using Tawtheef.Infrastructure.Extensions;

namespace Operations.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class RolesController(IMediator mediator) : ControllerBase
{
    [HttpGet("list-roles")]
    public async Task<IActionResult> ListRoles([FromQuery] ListRolesQuery query)
    {
        var result = await mediator.Send(query);
        return result.ToActionResult();
    }

    [HttpGet("role-details/{id:guid}")]
    public async Task<IActionResult> GetRole(Guid id)
    {
        var result = await mediator.Send(new GetRoleDetailsQuery(id));
        return result.ToActionResult();
    }

    [HttpPost("create-role")]
    public async Task<IActionResult> CreateRole([FromBody] CreateRoleCommand command)
    {
        var result = await mediator.Send(command);
        return result.ToActionResult();
    }

    [HttpPut("update-role/{id:guid}")]
    public async Task<IActionResult> UpdateRole(Guid id, [FromBody] UpdateRoleCommand command)
    {
        var result = await mediator.Send(command with { Id = id });
        return result.ToActionResult();
    }

    [HttpDelete("delete-role/{id:guid}")]
    public async Task<IActionResult> DeleteRole(Guid id)
    {
        var result = await mediator.Send(new DeleteRoleCommand(id));
        return result.ToActionResult();
    }

    [HttpGet("list-permissions")]
    public async Task<IActionResult> ListPermissions()
    {
        var result = await mediator.Send(new ListPermissionsQuery());
        return result.ToActionResult();
    }
}
