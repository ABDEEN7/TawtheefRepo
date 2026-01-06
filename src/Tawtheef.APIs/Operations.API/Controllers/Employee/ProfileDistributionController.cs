
using Cortex.Mediator;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Tawtheef.Application.Common;
using Tawtheef.Application.Common.Security;
using Tawtheef.Application.Common.Security.Tawtheef.Application.Common.Security;
using Tawtheef.Application.Features.Operations.Employee.ProfileManagement.ProfileDistribution.Commands;
using Tawtheef.Application.Features.Operations.Employee.ProfileManagement.ProfileDistribution.Queries;
using Tawtheef.Domain.Entities.Users;
using Tawtheef.Infrastructure.Extensions;

namespace Operations.API.Controllers.Employee;

[ApiController]
[Route("api/profile-distributions")]
[Microsoft.AspNetCore.Authorization.Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class ProfileDistributionController(IMediator mediator) : ControllerBase
{
    [HttpGet("profiles")]
    [AuthorizePermission(PermissionKeys.Profile.View)]
    public async Task<IActionResult> GetFiles([FromQuery] UserProfileStatus? status, CancellationToken ct)
    {
        var result = await mediator.Send(new GetDistributionProfilesQuery(status), ct);
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
        var command = new ManualAssignProfilesCommand(request.EmployeeId, request.ProfileIds);
        var result = await mediator.Send(command, ct);
        return result.ToActionResult();
    }

    [HttpPost("assign/auto")]
    [AuthorizePermission(PermissionKeys.Profile.Manage)]
    public async Task<IActionResult> AssignAutomatically([FromBody] AutoAssignRequest request, CancellationToken ct)
    {
        var command = new AutoAssignProfilesCommand(request.EmployeeIds, request.ProfileIds, request.PerEmployeeCount);
        var result = await mediator.Send(command, ct);
        return result.ToActionResult();
    }

    [HttpPost("reassign")]
    [AuthorizePermission(PermissionKeys.Profile.Manage)]
    public async Task<IActionResult> Reassign([FromBody] ReassignRequest request, CancellationToken ct)
    {
        var command = new ReassignProfilesCommand(
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
