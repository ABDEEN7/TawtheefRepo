using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Tawtheef.Application.Common.Constants;
using Tawtheef.Application.Features.Lookups.Queries;
using Tawtheef.Application.Features.Operations.Employee.JobInvitationSummary.Queries;
using Tawtheef.Infrastructure.Extensions;
using Tawtheef.Infrastructure.Services.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace Operations.API.Controllers.Employee;

[ApiController]
[Route("api/[controller]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
//[Authorize(Policy = PermissionPolicyProvider.POLICY_PREFIX + PermissionNames.JobsManage)]
public class JobInvitationSummaryController(IMediator mediator) : ControllerBase
{
    #region Lookups
    [HttpGet("lookups/job-categories")]
    public async Task<IActionResult> GetJobCategories()
    {
        var result = await mediator.Send(new GetJobCategoriesQuery());
        return result.ToActionResult();
    }

    [HttpGet("lookups/departments")]
    public async Task<IActionResult> GetDepartments()
    {
        var result = await mediator.Send(new GetDepartmentsQuery());
        return result.ToActionResult();
    }
    
    [HttpGet("lookups/job-statuses")]
    public async Task<IActionResult> GetJobStatusesQuery()
    {
        var result = await mediator.Send(new GetJobStatusesQuery());
        return result.ToActionResult();
    }
    #endregion
    
    #region Retrive Job Invitation Data
    [HttpGet("get-invitations-summary")]
    public async Task<IActionResult> GetInvitationsSummary([FromQuery] GetJobInvitationSummaryQuery query)
    {
        var result = await mediator.Send(query);
        return result.ToActionResult();
    }
    #endregion
}
