using Application.Operation.Features.Employee.OfficeUsers.Commands;
using Application.Operation.Features.Employee.OfficeUsers.Queries;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Tawtheef.Application.Common.Security;
using Tawtheef.Infrastructure.Extensions;

namespace Operations.API.Controllers.Employee;

[ApiController]
[Route("api/[controller]")]
[Microsoft.AspNetCore.Authorization.Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class OfficeUsersController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [AuthorizePermission(PermissionKeys.OfficeUsers.View)]
    public async Task<IActionResult> ListUsers([FromQuery] GetOfficeUsersQuery query)
    {
        var result = await mediator.Send(query);
        return result.ToActionResult();
    }

    [HttpPost]
    [AuthorizePermission(PermissionKeys.OfficeUsers.Manage)]
    public async Task<IActionResult> CreateUser([FromBody] CreateOfficeUserCommand command)
    {
        var result = await mediator.Send(command);
        return result.ToActionResult();
    }

    [HttpPut("{id:guid}")]
    [AuthorizePermission(PermissionKeys.OfficeUsers.Manage)]
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateOfficeUserCommand command)
    {
        var result = await mediator.Send(command with { UserId = id });
        return result.ToActionResult();
    }

    [HttpPut("{id:guid}/block-status")]
    [AuthorizePermission(PermissionKeys.OfficeUsers.Manage)]
    public async Task<IActionResult> UpdateUserBlockStatus(
        Guid id,
        [FromBody] UpdateOfficeUserBlockStatusCommand command)
    {
        var result = await mediator.Send(command with { UserId = id });
        return result.ToActionResult();
    }
}

