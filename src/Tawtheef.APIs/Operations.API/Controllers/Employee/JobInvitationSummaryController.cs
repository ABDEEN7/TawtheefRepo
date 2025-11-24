using MediatR;
using Microsoft.AspNetCore.Mvc;
using Tawtheef.Application.Features.Lookups.Queries;
using Tawtheef.Application.Features.Operations.Employee.JobInvitationSummary.Queries;
using Tawtheef.Infrastructure.Extensions;

namespace Operations.API.Controllers.Employee;

[ApiController]
[Route("api/[controller]")]
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
    [HttpGet("get-candidate-invitations")]
    public async Task<IActionResult> GetCandidateInvitations([FromBody] GetJobInvitationSummaryQuery query)
    {
        var result = await mediator.Send(query);
        return result.ToActionResult();
    }
    #endregion
}
