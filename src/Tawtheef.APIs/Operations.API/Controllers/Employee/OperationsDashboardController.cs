using System.Security.Claims;
using Application.Operation.Features.Employee.Dashboard.Queries;
using MediatR;
using FluentResults;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Tawtheef.Application.Common.Security;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Users;
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
}

