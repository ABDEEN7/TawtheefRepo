using Application.Operation.Features.Employee.Interview.EvaluationBank.Commands;
using Application.Operation.Features.Employee.Interview.EvaluationBank.Handlers.Queries;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Tawtheef.Application.Common.Security;
using Tawtheef.Infrastructure.Extensions;


namespace Operations.API.Controllers.Interview;

[Route("api/[controller]")]
[ApiController]
[Microsoft.AspNetCore.Authorization.Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class InterviewEvaluationBankController(IMediator mediator) : ControllerBase
{
    [HttpGet("axes")]
    [AuthorizePermission(PermissionKeys.InterviewEvaluationBank.View, PermissionKeys.InterviewEvaluationBank.Manage)]
    public async Task<IActionResult> ListActiveAxes([FromQuery] ListActiveAxesQuery query, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPost("axes")]
    [AuthorizePermission(PermissionKeys.InterviewEvaluationBank.Manage)]
    public async Task<IActionResult> CreateAxis([FromBody] CreateAxisCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPut("axes")]
    [AuthorizePermission(PermissionKeys.InterviewEvaluationBank.Manage)]
    public async Task<IActionResult> UpdateAxis([FromBody] UpdateAxisCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPut("axes/change-activation")]
    [AuthorizePermission(PermissionKeys.InterviewEvaluationBank.Manage)]
    public async Task<IActionResult> ChangeAxisActivation([FromBody] ChangeAxisActivationCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return result.ToActionResult();
    }

    [HttpGet("criteria")]
    [AuthorizePermission(PermissionKeys.InterviewEvaluationBank.View, PermissionKeys.InterviewEvaluationBank.Manage)]
    public async Task<IActionResult> ListCriteriaByAxis([FromQuery] ListCriteriaByAxisQuery query, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPost("criteria")]
    [AuthorizePermission(PermissionKeys.InterviewEvaluationBank.Manage)]
    public async Task<IActionResult> CreateCriterion([FromBody] CreateCriterionCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPut("criteria")]
    [AuthorizePermission(PermissionKeys.InterviewEvaluationBank.Manage)]
    public async Task<IActionResult> UpdateCriterion([FromBody] UpdateCriterionCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPut("criteria/change-activation")]
    [AuthorizePermission(PermissionKeys.InterviewEvaluationBank.Manage)]
    public async Task<IActionResult> ChangeCriterionActivation([FromBody] ChangeCriterionActivationCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return result.ToActionResult();
    }
}
