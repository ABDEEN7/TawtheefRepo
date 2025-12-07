using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tawtheef.Application.Features.Lookups.Queries;
using Tawtheef.Application.Features.Recruitment.Dashboard.DTOs;
using Tawtheef.Application.Features.Recruitment.Dashboard.Queries;
using Tawtheef.Infrastructure.Extensions;

namespace Recruitment.API.Controllers.Applicant;

[ApiController]
[Route("api/[controller]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class DashboardController(IMediator mediator) : ControllerBase
{
    #region Lookups
    [HttpGet("lookups/invitation-statuses")]
    public async Task<IActionResult> GetInvitationStatuses()
    {
        var result = await mediator.Send(new GetInvitationStatusesQuery());
        return result.ToActionResult();
    }
    
    [HttpGet("lookups/job-categories")]
    public async Task<IActionResult> GetJobCategories()
    {
        var result = await mediator.Send(new GetJobCategoriesQuery());
        return result.ToActionResult();
    }
    
    [HttpGet("lookups/department")]
    public async Task<IActionResult> GetDepartments()
    {
        var result = await mediator.Send(new GetDepartmentsQuery());
        return result.ToActionResult();
    }
    #endregion

    #region Retrive Dashboard Data
    [HttpGet("get-candidate-invitations")]
    public async Task<IActionResult> GetCandidateInvitations([FromQuery] GetCandidateInvitationsQuery query)
    {
        var result = await mediator.Send(query);
        return result.ToActionResult();
    }
    #endregion
}
