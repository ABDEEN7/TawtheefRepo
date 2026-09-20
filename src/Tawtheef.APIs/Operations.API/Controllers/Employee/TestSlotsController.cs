using Application.Operation.Features.Employee.TestSlots.Commands;
using Application.Operation.Features.Employee.TestSlots.DTOs;
using Application.Operation.Features.Employee.TestSlots.Queries;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Tawtheef.Application.Common.Security;
using Tawtheef.Infrastructure.Extensions;

namespace Operations.API.Controllers.Employee;

[ApiController]
[Route("api/test-slots")]
[Microsoft.AspNetCore.Authorization.Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public sealed class TestSlotsController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [AuthorizePermission(PermissionKeys.TestSlots.Create)]
    public async Task<IActionResult> SaveTestSlot([FromBody] CreateTestSlotDto testSlot,
        CancellationToken cancellationToken)
        => (await mediator.Send(new SaveTestSlotCommand(null, testSlot), cancellationToken)).ToActionResult();

    [HttpPut("{id:guid}")]
    [AuthorizePermission(PermissionKeys.TestSlots.Create)]
    public async Task<IActionResult> UpdateTestSlot(Guid id, [FromBody] CreateTestSlotDto testSlot,
        CancellationToken cancellationToken)
        => (await mediator.Send(new SaveTestSlotCommand(id, testSlot), cancellationToken)).ToActionResult();

    [HttpGet]
    [AuthorizePermission(PermissionKeys.TestSlots.View)]
    public async Task<IActionResult> GetTestSlots([FromQuery] GetTestSlotsQuery query,
        CancellationToken cancellationToken)
        => (await mediator.Send(query, cancellationToken)).ToActionResult();

    [HttpGet("lookups/rooms")]
    [AuthorizePermission(PermissionKeys.TestSlots.View, PermissionKeys.TestSlots.Create)]
    public async Task<IActionResult> GetAvailableRoomsForTestSlot([FromQuery] string language,
        CancellationToken cancellationToken)
        => (await mediator.Send(new GetAvailableRoomsForTestSlotQuery(language), cancellationToken))
            .ToActionResult();

    [HttpGet("wizard/staff")]
    [AuthorizePermission(PermissionKeys.TestSlots.Create)]
    public async Task<IActionResult> GetStaffMembers([FromQuery] GetTestSlotStaffMembersQuery query,
        CancellationToken cancellationToken)
        => (await mediator.Send(query, cancellationToken)).ToActionResult();

    [HttpGet("{id:guid}/configuration")]
    [AuthorizePermission(PermissionKeys.TestSlots.View, PermissionKeys.TestSlots.Create)]
    public async Task<IActionResult> GetConfiguration(Guid id, [FromQuery] string language,
        CancellationToken cancellationToken)
        => (await mediator.Send(new GetTestSlotConfigurationQuery(id, language), cancellationToken)).ToActionResult();

    [HttpGet("{id:guid}")]
    [AuthorizePermission(PermissionKeys.TestSlots.View)]
    public async Task<IActionResult> GetDetails(Guid id, [FromQuery] string language, CancellationToken cancellationToken)
        => (await mediator.Send(new GetTestSlotDetailsQuery(id, language), cancellationToken)).ToActionResult();

    [HttpGet("{id:guid}/sessions")]
    [AuthorizePermission(PermissionKeys.TestSlots.View)]
    public async Task<IActionResult> GetSessions(Guid id, [FromQuery] string language, CancellationToken cancellationToken)
        => (await mediator.Send(new GetTestSlotSessionsQuery(id, language), cancellationToken)).ToActionResult();

    [HttpGet("{id:guid}/candidates")]
    [AuthorizePermission(PermissionKeys.TestSlots.View)]
    public async Task<IActionResult> GetCandidates(Guid id, [FromQuery] GetTestSlotCandidatesQuery query,
        CancellationToken cancellationToken)
        => (await mediator.Send(query with { TestSlotId = id }, cancellationToken)).ToActionResult();

    [HttpGet("{id:guid}/access-code")]
    [AuthorizePermission(PermissionKeys.TestSlots.ViewAccessCode)]
    public async Task<IActionResult> GetAccessCode(Guid id, CancellationToken cancellationToken)
        => (await mediator.Send(new GetTestSlotAccessCodeQuery(id), cancellationToken)).ToActionResult();
}
