using Application.Operation.Features.Employee.TestSessions.Queries;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Tawtheef.Application.Common.Security;
using Tawtheef.Infrastructure.Extensions;

namespace Operations.API.Controllers.Employee;

[ApiController]
[Route("api/test-sessions")]
[Microsoft.AspNetCore.Authorization.Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public sealed class TestSessionsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [AuthorizePermission(PermissionKeys.TestSessions.View)]
    public async Task<IActionResult> List([FromQuery] ListTestSessionsQuery query, CancellationToken ct)
        => (await mediator.Send(query, ct)).ToActionResult();

    [HttpGet("lookups")]
    [AuthorizePermission(PermissionKeys.TestSessions.View)]
    public async Task<IActionResult> Lookups(
        [FromQuery] string language,
        [FromQuery] Guid? roomId,
        CancellationToken ct)
        => (await mediator.Send(new GetTestSessionLookupsQuery(language, roomId), ct)).ToActionResult();
}
