
using Cortex.Mediator;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Tawtheef.Application.Common.Security;
using Tawtheef.Application.Common.Security.Tawtheef.Application.Common.Security;
using Tawtheef.Application.Features.Operations.Admin.Religions.Commands;
using Tawtheef.Application.Features.Operations.Admin.Religions.Queries;
using Tawtheef.Infrastructure.Extensions;

namespace Operations.API.Controllers.Admin;

[ApiController]
[Route("api/[controller]")]
[Microsoft.AspNetCore.Authorization.Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class ReligionsController(IMediator mediator) : ControllerBase
{
    [HttpGet("list-religions")]
    [AuthorizePermission(PermissionKeys.Religions.View)]
    public async Task<IActionResult> ListReligions([FromQuery] GetListReligionsQuery query)
    {
        var result = await mediator.Send(query);
        return result.ToActionResult();
    }

    [HttpGet("religion-details/{id:guid}")]
    [AuthorizePermission(PermissionKeys.Religions.View)]
    public async Task<IActionResult> ReligionDetails(Guid id)
    {
        var result = await mediator.Send(new GetReligionDetailsQuery(id));
        return result.ToActionResult();
    }

    [HttpPost("create-religion")]
    [AuthorizePermission(PermissionKeys.Religions.Manage)]
    public async Task<IActionResult> CreateReligion([FromBody] CreateReligionCommand command)
    {
        var result = await mediator.Send(command);
        return result.ToActionResult();
    }

    [HttpPut("update-religion/{id:guid}")]
    [AuthorizePermission(PermissionKeys.Religions.Manage)]
    public async Task<IActionResult> UpdateReligion(Guid id, [FromBody] UpdateReligionCommand command)
    {
        var result = await mediator.Send(command with { Id = id });
        return result.ToActionResult();
    }

    [HttpPut("{id:guid}/status")]
    [AuthorizePermission(PermissionKeys.Religions.Manage)]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateReligionStatusCommand command)
    {
        var result = await mediator.Send(command with { ReligionId = id });
        return result.ToActionResult();
    }
}
