using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tawtheef.Application.Common.Constants;
using Tawtheef.Application.Features.Lookups.Queries;
using Tawtheef.Application.Features.Operations.Employee.JobCandidates.Commands;
using Tawtheef.Application.Features.Operations.Employee.JobCandidates.Queries;
using Tawtheef.Infrastructure.Extensions;
using Tawtheef.Infrastructure.Services.Authorization;

namespace Operations.API.Controllers.Employee;

[ApiController]
[Route("api/[controller]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public sealed class JobCandidatesController(IMediator mediator) : ControllerBase
{
    #region Lookups
    [HttpGet("lookups/candidate-types")]
    [Authorize(Policy = PermissionPolicyProvider.PolicyPrefix + PermissionNames.JobsManage)]
    public async Task<IActionResult> GetCandidateTypes()
    {
        var result = await mediator.Send(new GetCandidateTypesQuery());
        return result.ToActionResult();
    }
    #endregion

    #region Job Candidates
    [HttpGet("overview")]
    [Authorize(Policy = PermissionPolicyProvider.PolicyPrefix + PermissionNames.JobsManage)]
    public async Task<IActionResult> GetOverview([FromQuery] GetJobCandidatesOverviewQuery query)
    {
        var result = await mediator.Send(query);
        return result.ToActionResult();
    }

    [HttpPost("search")]
    [Authorize(Policy = PermissionPolicyProvider.PolicyPrefix + PermissionNames.JobsManage)]
    public async Task<IActionResult> Search([FromBody] GetJobCandidatesQuery query)
    {
        var result = await mediator.Send(query);
        return result.ToActionResult();
    }

    [HttpPost("export")]
    [Authorize(Policy = PermissionPolicyProvider.PolicyPrefix + PermissionNames.JobsManage)]
    public async Task<IActionResult> Export([FromBody] ExportJobCandidatesQuery query)
    {
        var result = await mediator.Send(query);
        if (result.IsFailed)
        {
            return result.ToActionResult();
        }

        var payload = result.Value;
        return File(payload.Content, payload.ContentType, payload.FileName);
    }

    [HttpPost("send-invitations")]
    [Authorize(Policy = PermissionPolicyProvider.PolicyPrefix + PermissionNames.JobsManage)]
    public async Task<IActionResult> SendInvitations([FromBody] SendJobCandidateInvitationsCommand command)
    {
        var result = await mediator.Send(command);
        return result.ToActionResult();
    }
    #endregion
}
