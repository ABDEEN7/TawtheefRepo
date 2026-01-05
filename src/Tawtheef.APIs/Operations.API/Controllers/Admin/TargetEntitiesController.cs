
using Cortex.Mediator;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Tawtheef.Application.Common;
using Tawtheef.Application.Common.Security;
using Tawtheef.Application.Common.Security.Tawtheef.Application.Common.Security;
using Tawtheef.Application.Features.Operations.Admin.TargetEntities.Commands;
using Tawtheef.Application.Features.Operations.Admin.TargetEntities.Queries;
using Tawtheef.Infrastructure.Extensions;

namespace Operations.API.Controllers.Admin;

[ApiController]
[Route("api/[controller]")]
[Microsoft.AspNetCore.Authorization.Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class TargetEntitiesController(IMediator mediator) : ControllerBase
{
    [HttpGet("list-target-entities")]
    [AuthorizePermission(PermissionKeys.TargetEntities.View)]
    public async Task<IActionResult> ListTargetEntities([FromQuery] GetListTargetEntitiesQuery query)
    {
        var result = await mediator.Send(query);
        return result.ToActionResult();
    }

    [HttpGet("target-entity-details/{id:guid}")]
    [AuthorizePermission(PermissionKeys.TargetEntities.View)]
    public async Task<IActionResult> TargetEntityDetails(Guid id)
    {
        var result = await mediator.Send(new GetTargetEntityDetailsQuery(id));
        return result.ToActionResult();
    }

    [HttpPost("create-target-entity")]
    [AuthorizePermission(PermissionKeys.TargetEntities.Manage)]
    public async Task<IActionResult> CreateTargetEntity([FromBody] CreateTargetEntityCommand command)
    {
        var result = await mediator.Send(command);
        return result.ToActionResult();
    }

    [HttpPut("update-target-entity/{id:guid}")]
    [AuthorizePermission(PermissionKeys.TargetEntities.Manage)]
    public async Task<IActionResult> UpdateTargetEntity(Guid id, [FromBody] UpdateTargetEntityCommand command)
    {
        var result = await mediator.Send(command with { Id = id });
        return result.ToActionResult();
    }

    [HttpPut("{id:guid}/status")]
    [AuthorizePermission(PermissionKeys.TargetEntities.Manage)]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateTargetEntityStatusCommand command)
    {
        var result = await mediator.Send(command with { TargetEntityId = id });
        return result.ToActionResult();
    }
}
