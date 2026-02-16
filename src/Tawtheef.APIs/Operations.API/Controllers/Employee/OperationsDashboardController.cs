using System.Security.Claims;
using Application.Operation.Features.Employee.Dashboard.Queries;
using Cortex.Mediator;
using FluentResults;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Tawtheef.Application.Common;
using Tawtheef.Application.Common.Security;
using Tawtheef.Domain.Entities.Users;
using Tawtheef.Infrastructure.Extensions;

namespace Operations.API.Controllers.Employee;

[ApiController]
[Route("api/operations-dashboard")]
[Microsoft.AspNetCore.Authorization.Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class OperationsDashboardController(IMediator mediator) : ControllerBase
{
    private Result<Guid> UserId => User.FindFirst(ClaimTypes.NameIdentifier)?.Value switch
    {
        null => Result.Fail<Guid>(ErrorsCodes.InvalidUserIdentifier),
        var id => Result.Ok(Guid.Parse(id))
    };

    [HttpGet]
    [AuthorizePermission(PermissionKeys.Dashboard.View)]
    public async Task<IActionResult> Get([FromQuery] GetOperationsDashboardQuery request, CancellationToken ct)
    {
        if (UserId.IsFailed) return BadRequest(UserId.Errors);

        var role = ResolveRole();
        var query = request with { CurrentUserId = UserId.Value, CurrentRole = role };
        var result = await mediator.Send(query, ct);
        return result.ToActionResult();
    }

    private string ResolveRole()
    {
        if (User.IsInRole(nameof(SystemRoleIds.HrManager)))
            return nameof(SystemRoleIds.HrManager);

        if (User.IsInRole(nameof(SystemRoleIds.DepartmentManager)))
            return nameof(SystemRoleIds.DepartmentManager);

        return nameof(SystemRoleIds.Employee);
    }
}
