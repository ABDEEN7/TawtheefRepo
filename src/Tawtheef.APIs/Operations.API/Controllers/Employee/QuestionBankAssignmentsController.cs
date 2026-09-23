using Application.Operation.Features.Employee.QuestionBankAssignments.Commands;
using Application.Operation.Features.Employee.QuestionBankAssignments.Queries;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Tawtheef.Application.Common.Security;
using Tawtheef.Infrastructure.Extensions;

namespace Operations.API.Controllers.Employee;

[ApiController]
[Route("api/[controller]")]
[Microsoft.AspNetCore.Authorization.Authorize(
    AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[AuthorizePermission(PermissionKeys.QuestionBankAssignments.Manage)]
public sealed class QuestionBankAssignmentsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List(
        [FromQuery] ListMyQuestionBankAssignmentsQuery query,
        CancellationToken cancellationToken)
    {
        return (await mediator.Send(query, cancellationToken)).ToActionResult();
    }

    [HttpGet("{assignmentId:guid}")]
    public async Task<IActionResult> Workspace(
        Guid assignmentId,
        CancellationToken cancellationToken)
    {
        var query = new GetQuestionBankAssignmentWorkspaceQuery(assignmentId);
        return (await mediator.Send(query, cancellationToken)).ToActionResult();
    }

    [HttpPost("{assignmentId:guid}/questions")]
    public async Task<IActionResult> Add(
        Guid assignmentId,
        [FromBody] QuestionInput input,
        CancellationToken cancellationToken)
    {
        var command = new AddQuestionToAssignmentCommand(assignmentId, input);
        return (await mediator.Send(command, cancellationToken)).ToActionResult();
    }

    [HttpPut("{assignmentId:guid}/questions/{itemId:guid}")]
    public async Task<IActionResult> Edit(
        Guid assignmentId,
        Guid itemId,
        [FromBody] QuestionInput input,
        CancellationToken cancellationToken)
    {
        var command = new EditAssignmentQuestionCommand(assignmentId, itemId, input);
        return (await mediator.Send(command, cancellationToken)).ToActionResult();
    }

    [HttpDelete("{assignmentId:guid}/questions/{itemId:guid}")]
    public async Task<IActionResult> Remove(
        Guid assignmentId,
        Guid itemId,
        CancellationToken cancellationToken)
    {
        var command = new RemoveAssignmentQuestionCommand(assignmentId, itemId);
        return (await mediator.Send(command, cancellationToken)).ToActionResult();
    }

    [HttpPost("{assignmentId:guid}/finish")]
    public async Task<IActionResult> Finish(
        Guid assignmentId,
        CancellationToken cancellationToken)
    {
        var command = new FinishQuestionBankAssignmentEntryCommand(assignmentId);
        return (await mediator.Send(command, cancellationToken)).ToActionResult();
    }
}
