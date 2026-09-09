using Application.Operation.Features.Employee.Exams.Commands;
using Application.Operation.Features.Employee.Exams.DTOs;
using Application.Operation.Features.Employee.Exams.Queries;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Tawtheef.Application.Common.Security;
using Tawtheef.Application.Features.Lookups.Queries;
using Tawtheef.Infrastructure.Extensions;

namespace Operations.API.Controllers.Employee;

[ApiController]
[Route("api/[controller]")]
[Microsoft.AspNetCore.Authorization.Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public sealed class ExamsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [AuthorizePermission(PermissionKeys.Exams.View)]
    public async Task<IActionResult> List([FromQuery] ListExamsQuery query, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query, cancellationToken);
        return result.ToActionResult();
    }

    [HttpGet("lookups/statuses")]
    [AuthorizePermission(PermissionKeys.Exams.View)]
    public async Task<IActionResult> GetStatuses([FromQuery] GetExamStatusesQuery query,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query, cancellationToken);
        return result.ToActionResult();
    }

    [HttpGet("lookups/specializations")]
    [AuthorizePermission(PermissionKeys.Exams.View)]
    public async Task<IActionResult> GetSpecializations([FromQuery] GetExamSpecializationsQuery query,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query, cancellationToken);
        return result.ToActionResult();
    }
    
    [HttpGet("wizard/lookups")]
    [AuthorizePermission(PermissionKeys.Exams.Create)]
    public async Task<IActionResult> WizardLookups(CancellationToken ct)
        => (await mediator.Send(new GetExamLookupsQuery(), ct)).ToActionResult();

    [HttpGet("wizard/view/lookups")]
    [AuthorizePermission(PermissionKeys.Exams.View)]
    public async Task<IActionResult> ViewLookups(CancellationToken ct)
        => (await mediator.Send(new GetExamLookupsQuery(), ct)).ToActionResult();

    [HttpGet("wizard/jobs")]
    [AuthorizePermission(PermissionKeys.Exams.Create)]
    public async Task<IActionResult> WizardJobs([FromQuery] string? search, [FromQuery] Guid? includeJobId,
        CancellationToken ct)
        => (await mediator.Send(new GetExamJobsQuery(search, includeJobId), ct)).ToActionResult();

    [HttpGet("wizard/view/jobs")]
    [AuthorizePermission(PermissionKeys.Exams.View)]
    public async Task<IActionResult> ViewJobs([FromQuery] string? search, [FromQuery] Guid? includeJobId,
        CancellationToken ct)
        => (await mediator.Send(new GetExamJobsQuery(search, includeJobId), ct)).ToActionResult();

    [HttpGet("wizard/banks")]
    [AuthorizePermission(PermissionKeys.Exams.Create)]
    public async Task<IActionResult> WizardBanks([FromQuery] Guid jobId, CancellationToken ct)
        => (await mediator.Send(new GetExamBanksQuery(jobId), ct)).ToActionResult();

    [HttpGet("wizard/view/banks")]
    [AuthorizePermission(PermissionKeys.Exams.View)]
    public async Task<IActionResult> ViewBanks([FromQuery] Guid jobId, CancellationToken ct)
        => (await mediator.Send(new GetExamBanksQuery(jobId), ct)).ToActionResult();

    [HttpGet("wizard/existing")]
    [AuthorizePermission(PermissionKeys.Exams.View)]
    public async Task<IActionResult> Existing([FromQuery] Guid jobId, CancellationToken ct)
        => (await mediator.Send(new GetExistingExamQuery(jobId), ct)).ToActionResult();

    [HttpGet("{id:guid}/configuration")]
    [AuthorizePermission(PermissionKeys.Exams.Create)]
    public async Task<IActionResult> Configuration(Guid id, CancellationToken ct)
        => (await mediator.Send(new GetExamConfigurationQuery(id), ct)).ToActionResult();

    [HttpGet("{id:guid}/configuration/view")]
    [AuthorizePermission(PermissionKeys.Exams.View)]
    public async Task<IActionResult> ViewConfiguration(Guid id, CancellationToken ct)
        => (await mediator.Send(new GetExamConfigurationQuery(id, true), ct)).ToActionResult();

    [HttpPost]
    [AuthorizePermission(PermissionKeys.Exams.Create)]
    public async Task<IActionResult> Create([FromBody] ExamConfigurationDto exam,
        [FromQuery] bool submit, CancellationToken ct)
        => (await mediator.Send(new SaveExamCommand(null, exam, submit), ct)).ToActionResult();

    [HttpPut("{id:guid}")]
    [HttpPut("{id:guid}/draft")]
    [AuthorizePermission(PermissionKeys.Exams.Create)]
    public async Task<IActionResult> Update(Guid id, [FromBody] ExamConfigurationDto exam,
        [FromQuery] bool submit, CancellationToken ct)
        => (await mediator.Send(new SaveExamCommand(id, exam, submit), ct)).ToActionResult();

    [HttpPost("{id:guid}/return")]
    [AuthorizePermission(PermissionKeys.Exams.WorkflowActions)]
    public async Task<IActionResult> Return(Guid id, [FromBody] ReturnExamRequest request, CancellationToken ct)
        => (await mediator.Send(new ReturnExamCommand(id, request.Note), ct)).ToActionResult();

    public sealed record ReturnExamRequest(string? Note);
}
