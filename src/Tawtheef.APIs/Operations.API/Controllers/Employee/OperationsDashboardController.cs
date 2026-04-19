using Application.Operation.Features.Employee.Dashboard.Queries;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Tawtheef.Application.Common.Security;
using Tawtheef.Infrastructure.Extensions;

namespace Operations.API.Controllers.Employee;

[ApiController]
[Route("api/operations-dashboard")]
[Microsoft.AspNetCore.Authorization.Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class OperationsDashboardController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [AuthorizePermission(PermissionKeys.Dashboard.View)]
    public async Task<IActionResult> Get([FromQuery] GetOperationsDashboardQuery request, CancellationToken ct)
    {
        var result = await mediator.Send(request, ct);
        return result.ToActionResult();
    }

    [HttpGet("team-performance")]
    [AuthorizePermission(PermissionKeys.Dashboard.View)]
    public async Task<IActionResult> GetTeamPerformance([FromQuery] GetTeamPerformanceQuery request, CancellationToken ct)
    {
        var result = await mediator.Send(request, ct);
        return result.ToActionResult();
    }
}

