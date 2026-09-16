using Application.Operation.Features.Employee.Interview.EvaluationTemplate.Commands;
using Application.Operation.Features.Employee.Interview.EvaluationTemplate.Queries;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Tawtheef.Application.Common.Security;
using Tawtheef.Infrastructure.Extensions;

namespace Operations.API.Controllers.Interview;

[Route("api/[controller]")]
[ApiController]
[Microsoft.AspNetCore.Authorization.Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class InterviewEvaluationTemplateController(IMediator mediator) : ControllerBase
{
    // ---- Templates ----

    [HttpGet("templates")]
    [AuthorizePermission(PermissionKeys.InterviewEvaluationTemplate.View, PermissionKeys.InterviewEvaluationTemplate.Manage)]
    public async Task<IActionResult> ListTemplates([FromQuery] ListTemplatesQuery query, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPost("templates")]
    [AuthorizePermission(PermissionKeys.InterviewEvaluationTemplate.Manage)]
    public async Task<IActionResult> CreateTemplate([FromBody] CreateTemplateCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPut("templates")]
    [AuthorizePermission(PermissionKeys.InterviewEvaluationTemplate.Manage)]
    public async Task<IActionResult> UpdateTemplate([FromBody] UpdateTemplateCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPut("templates/change-activation")]
    [AuthorizePermission(PermissionKeys.InterviewEvaluationTemplate.Manage)]
    public async Task<IActionResult> ChangeTemplateActivation([FromBody] ChangeTemplateActivationCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return result.ToActionResult();
    }

    // ---- Versions ----

    [HttpGet("versions")]
    [AuthorizePermission(PermissionKeys.InterviewEvaluationTemplate.View, PermissionKeys.InterviewEvaluationTemplate.Manage)]
    public async Task<IActionResult> ListTemplateVersions([FromQuery] ListTemplateVersionsQuery query, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query, cancellationToken);
        return result.ToActionResult();
    }

    [HttpGet("versions/details")]
    [AuthorizePermission(PermissionKeys.InterviewEvaluationTemplate.View, PermissionKeys.InterviewEvaluationTemplate.Manage)]
    public async Task<IActionResult> GetTemplateVersionDetails([FromQuery] GetTemplateVersionDetailsQuery query, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPost("versions")]
    [AuthorizePermission(PermissionKeys.InterviewEvaluationTemplate.Manage)]
    public async Task<IActionResult> CreateTemplateVersion([FromBody] CreateTemplateVersionCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPut("versions")]
    [AuthorizePermission(PermissionKeys.InterviewEvaluationTemplate.Manage)]
    public async Task<IActionResult> UpdateTemplateVersion([FromBody] UpdateTemplateVersionCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPut("versions/submit")]
    [AuthorizePermission(PermissionKeys.InterviewEvaluationTemplate.Manage)]
    public async Task<IActionResult> SubmitTemplateVersion([FromBody] SubmitTemplateVersionCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPut("versions/approve")]
    [AuthorizePermission(PermissionKeys.InterviewEvaluationTemplate.Manage)]
    public async Task<IActionResult> ApproveTemplateVersion([FromBody] ApproveTemplateVersionCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPut("versions/return")]
    [AuthorizePermission(PermissionKeys.InterviewEvaluationTemplate.Manage)]
    public async Task<IActionResult> ReturnTemplateVersion([FromBody] ReturnTemplateVersionCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPut("versions/cancel")]
    [AuthorizePermission(PermissionKeys.InterviewEvaluationTemplate.Manage)]
    public async Task<IActionResult> CancelTemplateVersion([FromBody] CancelTemplateVersionCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return result.ToActionResult();
    }

    // ---- Version axes ----

    [HttpPost("versions/axes")]
    [AuthorizePermission(PermissionKeys.InterviewEvaluationTemplate.Manage)]
    public async Task<IActionResult> AddTemplateVersionAxis([FromBody] AddTemplateVersionAxisCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPut("versions/axes")]
    [AuthorizePermission(PermissionKeys.InterviewEvaluationTemplate.Manage)]
    public async Task<IActionResult> UpdateTemplateVersionAxis([FromBody] UpdateTemplateVersionAxisCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return result.ToActionResult();
    }

    [HttpDelete("versions/axes/{id:guid}")]
    [AuthorizePermission(PermissionKeys.InterviewEvaluationTemplate.Manage)]
    public async Task<IActionResult> RemoveTemplateVersionAxis(Guid id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new RemoveTemplateVersionAxisCommand(id), cancellationToken);
        return result.ToActionResult();
    }

    // ---- Version axis criteria ----

    [HttpPost("versions/axes/criteria")]
    [AuthorizePermission(PermissionKeys.InterviewEvaluationTemplate.Manage)]
    public async Task<IActionResult> AddTemplateVersionCriterion([FromBody] AddTemplateVersionCriterionCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPut("versions/axes/criteria")]
    [AuthorizePermission(PermissionKeys.InterviewEvaluationTemplate.Manage)]
    public async Task<IActionResult> UpdateTemplateVersionCriterion([FromBody] UpdateTemplateVersionCriterionCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return result.ToActionResult();
    }

    [HttpDelete("versions/axes/criteria/{id:guid}")]
    [AuthorizePermission(PermissionKeys.InterviewEvaluationTemplate.Manage)]
    public async Task<IActionResult> RemoveTemplateVersionCriterion(Guid id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new RemoveTemplateVersionCriterionCommand(id), cancellationToken);
        return result.ToActionResult();
    }
}
