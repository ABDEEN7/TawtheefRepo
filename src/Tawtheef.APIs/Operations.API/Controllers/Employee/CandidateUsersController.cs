using Cortex.Mediator;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Tawtheef.Application.Common;
using Tawtheef.Application.Common.Security;
using Tawtheef.Application.Common.Security.Tawtheef.Application.Common.Security;
using Tawtheef.Application.Features.Operations.Employee.CandidateUsers.Commands;
using Tawtheef.Application.Features.Operations.Employee.CandidateUsers.Queries;
using Tawtheef.Infrastructure.Extensions;

namespace Operations.API.Controllers.Employee;

[ApiController]
[Route("api/[controller]")]
[Microsoft.AspNetCore.Authorization.Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class CandidateUsersController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [AuthorizePermission(PermissionKeys.Users.View)]
    public async Task<IActionResult> ListUsers([FromQuery] GetCandidateUsersQuery query)
    {
        var result = await mediator.Send(query);
        return result.ToActionResult();
    }

    [HttpPut("{id:guid}/block-status")]
    [AuthorizePermission(PermissionKeys.Users.Manage)]
    public async Task<IActionResult> UpdateUserBlockStatus(
        Guid id,
        [FromBody] UpdateCandidateUserBlockStatusCommand command)
    {
        var result = await mediator.Send(command with { UserId = id });
        return result.ToActionResult();
    }
}
