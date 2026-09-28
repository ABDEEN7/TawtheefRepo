using Application.Operation.Features.Employee.QuestionBankAssignments.Commands;
using Application.Operation.Features.Employee.QuestionBankAssignments.Queries;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Tawtheef.Application.Common.Security;
using Tawtheef.Infrastructure.Extensions;
using Tawtheef.Application.Common.Interfaces.Services.Security;
using Tawtheef.Application.Features.Resources.Commands;
using Tawtheef.Application.Common.Services;

namespace Operations.API.Controllers.Employee;

[ApiController]
[Route("api/[controller]")]
[Microsoft.AspNetCore.Authorization.Authorize(
    AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[AuthorizePermission(PermissionKeys.QuestionBankAssignments.Manage)]
public sealed class QuestionBankAssignmentsController(IMediator mediator, ICurrentUserService currentUser) : ControllerBase
{
    [HttpPost("images")]
    [RequestSizeLimit(Tawtheef.Domain.Constants.ProfileLimits.MaxExperienceFileSizeBytes + 65_536)]
    public async Task<IActionResult> UploadImage(IFormFile file, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(currentUser.UserId, out var userId)) return Unauthorized();
        var uploadPath = await QuestionImageUploadPathFactory.CreateAsync(userId, file, cancellationToken);
        var result = await mediator.Send(
            new UploadAttachmentCommand(
                userId,
                uploadPath.FileId,
                uploadPath.Path,
                uploadPath.Hash,
                file),
            cancellationToken);
        return result.ToActionResult();
    }
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
