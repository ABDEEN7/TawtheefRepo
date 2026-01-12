using System.Security.Claims;
using Application.Operation.Features.Employee.ProfileManagement.ProfileDistribution.Commands;
using Application.Operation.Features.Employee.ProfileManagement.ProfileDistribution.Queries;
using Cortex.Mediator;
using FluentResults;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Tawtheef.Application.Common;
using Tawtheef.Application.Common.Security;
using Tawtheef.Domain.Constants;
using Tawtheef.Infrastructure.Extensions;

namespace Operations.API.Controllers.Employee;

[ApiController]
[Route("api/profile-distributions")]
[Microsoft.AspNetCore.Authorization.Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class ProfileDistributionController(IMediator mediator) : ControllerBase
{
    private Result<Guid> UserId => User.FindFirst(ClaimTypes.NameIdentifier)?.Value switch
    {
        null => Result.Fail<Guid>(ErrorsCodes.InvalidUserIdentifier),
        var id => Result.Ok(Guid.Parse(id))
    };

    [HttpGet("profiles")]
    [AuthorizePermission(PermissionKeys.Profile.View)]
    public async Task<IActionResult> GetFiles([FromQuery] GetDistributionProfilesQuery query, CancellationToken ct)
    {
        if (UserId.IsFailed) return BadRequest(UserId.Errors);
        var result = await mediator.Send(query with {UserId = UserId.Value} , ct);
        return result.ToActionResult();
    }

    [HttpGet("employees")]
    [AuthorizePermission(PermissionKeys.Profile.View)]
    public async Task<IActionResult> GetEmployees(CancellationToken ct)
    {
        var result = await mediator.Send(new GetDistributionEmployeesQuery(), ct);
        return result.ToActionResult();
    }

    [HttpPost("assign/manual")]
    [AuthorizePermission(PermissionKeys.Profile.Manage)]
    public async Task<IActionResult> AssignManually([FromBody] ManualAssignRequest request, CancellationToken ct)
    {
        if (UserId.IsFailed) return BadRequest(UserId.Errors);
        var command = new ManualAssignProfilesCommand(UserId.Value, request.EmployeeId, request.ProfileIds);
        var result = await mediator.Send(command, ct);
        return result.ToActionResult();
    }

    [HttpPost("assign/auto")]
    [AuthorizePermission(PermissionKeys.Profile.Manage)]
    public async Task<IActionResult> AssignAutomatically([FromBody] AutoAssignRequest request, CancellationToken ct)
    {
        if (UserId.IsFailed) return BadRequest(UserId.Errors);
        var command = new AutoAssignProfilesCommand(UserId.Value, request.EmployeeIds, request.ProfileIds, request.PerEmployeeCount);
        var result = await mediator.Send(command, ct);
        return result.ToActionResult();
    }

    [HttpPost("reassign")]
    [AuthorizePermission(PermissionKeys.Profile.Manage)]
    public async Task<IActionResult> Reassign([FromBody] ReassignRequest request, CancellationToken ct)
    {
        if (UserId.IsFailed) return BadRequest(UserId.Errors);
        var command = new ReassignProfilesCommand(
            UserId.Value,
            request.Mode,
            request.EmployeeId,
            request.EmployeeIds,
            request.ProfileIds,
            request.PerEmployeeCount);

        var result = await mediator.Send(command, ct);
        return result.ToActionResult();
    }
}

public sealed class ManualAssignRequest
{
    public Guid EmployeeId { get; set; }
    public List<Guid> ProfileIds { get; set; } = [];
}

public sealed class AutoAssignRequest
{
    public List<Guid> EmployeeIds { get; set; } = [];
    public List<Guid>? ProfileIds { get; set; }
    public int? PerEmployeeCount { get; set; }
}

public sealed class ReassignRequest
{
    public string Mode { get; set; } = "manual";
    public Guid? EmployeeId { get; set; }
    public List<Guid> EmployeeIds { get; set; } = [];
    public List<Guid> ProfileIds { get; set; } = [];
    public int? PerEmployeeCount { get; set; }
}
