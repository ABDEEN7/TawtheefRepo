using Application.Operation.Features.Employee.TestSessions.DTOs;
using Application.Operation.Features.Employee.TestSessions.Commands;
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

    [HttpGet("wizard/exams/{examId:guid}")]
    [AuthorizePermission(PermissionKeys.TestSessions.Create)]
    public async Task<IActionResult> ExamDetails(Guid examId, [FromQuery] string language, CancellationToken ct)
        => (await mediator.Send(new GetTestSessionExamDetailsQuery(examId, language), ct)).ToActionResult();

    [HttpGet("wizard/candidates")]
    [AuthorizePermission(PermissionKeys.TestSessions.Create)]
    public async Task<IActionResult> Candidates([FromQuery] GetTestSessionCandidatesQuery query, CancellationToken ct)
        => (await mediator.Send(query, ct)).ToActionResult();

    [HttpGet("wizard/{testSessionId:guid}/edit")]
    [AuthorizePermission(PermissionKeys.TestSessions.Create)]
    public async Task<IActionResult> Edit(
        Guid testSessionId,
        [FromQuery] string language,
        CancellationToken ct)
        => (await mediator.Send(new GetTestSessionForEditQuery(testSessionId, language), ct)).ToActionResult();

    [HttpGet("wizard/ready-test-slots")]
    [AuthorizePermission(PermissionKeys.TestSessions.Create)]
    public async Task<IActionResult> ReadyTestSlots([FromQuery] GetReadyTestSlotsQuery query, CancellationToken ct)
        => (await mediator.Send(query, ct)).ToActionResult();

    [HttpGet("wizard/capacity")]
    [AuthorizePermission(PermissionKeys.TestSessions.Create)]
    public async Task<IActionResult> Capacity([FromQuery] GetTestSessionCapacityQuery query, CancellationToken ct)
        => (await mediator.Send(query, ct)).ToActionResult();

    [HttpPut("wizard/session-setup")]
    [AuthorizePermission(PermissionKeys.TestSessions.Create)]
    public async Task<IActionResult> SaveSessionSetup(
        [FromBody] SaveTestSessionSetupDto setup,
        CancellationToken ct)
        => (await mediator.Send(new SaveTestSessionSetupCommand(setup), ct)).ToActionResult();
}
