using Application.Operation.Features.Employee.CandidateUsers.Commands;
using Application.Operation.Features.Employee.CandidateUsers.Queries;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Tawtheef.Application.Common.Security;
using Tawtheef.Infrastructure.Extensions;

namespace Operations.API.Controllers.Employee;

[ApiController]
[Route("api/[controller]")]
[Microsoft.AspNetCore.Authorization.Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class CandidateUsersController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [AuthorizePermission(PermissionKeys.CandidateUsers.View)]
    public async Task<IActionResult> ListUsers([FromQuery] GetCandidateUsersQuery query)
    {
        var result = await mediator.Send(query);
        return result.ToActionResult();
    }

    [HttpPut("{id:guid}/block-status")]
    [AuthorizePermission(PermissionKeys.CandidateUsers.Manage)]
    public async Task<IActionResult> UpdateUserBlockStatus(
        Guid id,
        [FromBody] UpdateCandidateUserBlockStatusCommand command)
    {
        var result = await mediator.Send(command with { UserId = id });
        return result.ToActionResult();
    }

    [HttpGet("{id:guid}/profile")]
    [AuthorizePermission(PermissionKeys.CandidateUsers.View)]
    public async Task<IActionResult> GetCandidateUserProfile(Guid id)
    {
        var result = await mediator.Send(new GetCandidateUserProfileQuery(id));
        return result.ToActionResult();
    }

    [HttpGet("{id:guid}/profile-logs")]
    [AuthorizePermission(PermissionKeys.CandidateUsers.View)]
    public async Task<IActionResult> GetCandidateUserProfileLogs(Guid id, [FromQuery] GetCandidateUserProfileLogsQuery query)
    {
        var result = await mediator.Send(query with { UserId = id });
        return result.ToActionResult();
    }
}

