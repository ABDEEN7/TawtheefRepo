using Application.Operation.Features.Employee.Interview.Committee.Commands;
using Application.Operation.Features.Employee.Interview.Committee.Queries;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Tawtheef.Application.Common.Security;
using Tawtheef.Infrastructure.Extensions;

namespace Operations.API.Controllers.Interview;

[Route("api/[controller]")]
[ApiController]
[Microsoft.AspNetCore.Authorization.Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class InterviewCommitteeController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [AuthorizePermission(PermissionKeys.InterviewCommittee.View, PermissionKeys.InterviewCommittee.Manage)]
    public async Task<IActionResult> ListCommittees([FromQuery] ListCommitteesQuery query, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query, cancellationToken);
        return result.ToActionResult();
    }

    [HttpGet("{id:guid}")]
    [AuthorizePermission(PermissionKeys.InterviewCommittee.View, PermissionKeys.InterviewCommittee.Manage)]
    public async Task<IActionResult> GetCommitteeById(Guid id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetCommitteeByIdQuery(id), cancellationToken);
        return result.ToActionResult();
    }

    [HttpPost]
    [AuthorizePermission(PermissionKeys.InterviewCommittee.Manage)]
    public async Task<IActionResult> CreateCommittee([FromBody] CreateCommitteeCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPut]
    [AuthorizePermission(PermissionKeys.InterviewCommittee.Manage)]
    // if we need special permission for edit after approve we can add it in permissions
    //and make the logic in handler(cjeck user have role for edit after approve),
    public async Task<IActionResult> UpdateCommittee([FromBody] UpdateCommitteeCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPut("submit")]
    [AuthorizePermission(PermissionKeys.InterviewCommittee.Manage)]
    public async Task<IActionResult> SubmitCommittee([FromBody] SubmitCommitteeCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPut("approve")]
    [AuthorizePermission(PermissionKeys.InterviewCommittee.Manage)]
    public async Task<IActionResult> ApproveCommittee([FromBody] ApproveCommitteeCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPut("return")]
    [AuthorizePermission(PermissionKeys.InterviewCommittee.Manage)]
    public async Task<IActionResult> ReturnCommittee([FromBody] ReturnCommitteeCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPut("cancel")]
    [AuthorizePermission(PermissionKeys.InterviewCommittee.Manage)]
    public async Task<IActionResult> CancelCommittee([FromBody] CancelCommitteeCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPut("stop")]
    [AuthorizePermission(PermissionKeys.InterviewCommittee.Manage)]
    public async Task<IActionResult> StopCommittee([FromBody] StopCommitteeCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPut("reactivate")]
    [AuthorizePermission(PermissionKeys.InterviewCommittee.Manage)]
    public async Task<IActionResult> ReactivateCommittee([FromBody] ReactivateCommitteeCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPut("close")]
    [AuthorizePermission(PermissionKeys.InterviewCommittee.Manage)]
    public async Task<IActionResult> CloseCommittee([FromBody] CloseCommitteeCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return result.ToActionResult();
    }


    // ---- Members ----

    [HttpGet("members")]
    [AuthorizePermission(PermissionKeys.InterviewCommittee.View, PermissionKeys.InterviewCommittee.Manage)]
    public async Task<IActionResult> ListCommitteeMembers([FromQuery] ListCommitteeMembersQuery query, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query, cancellationToken);
        return result.ToActionResult();
    }

    // ---- Lookups ----

    // Custom endpoint to get users who are eligible to be committee members based on their roles and permissions
    [HttpGet("lookups/eligible-members")]
    [AuthorizePermission(PermissionKeys.InterviewCommittee.Manage)] // can be customs permission also, i put it as mamnge permission for now
    public async Task<IActionResult> ListEligibleMembers([FromQuery] ListEligibleCommitteeMembersQuery query, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query, cancellationToken);
        return result.ToActionResult();
    }
}
