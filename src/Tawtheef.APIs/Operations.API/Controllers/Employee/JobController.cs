using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tawtheef.Application.Common.Constants;
using Tawtheef.Application.Features.Lookups.Queries;
using Tawtheef.Application.Features.Operations.Employee.Job.Commands;
using Tawtheef.Application.Features.Operations.Employee.Job.Queries;
using Tawtheef.Infrastructure.Extensions;
using Tawtheef.Infrastructure.Services.Authorization;

namespace Operations.API.Controllers.Employee;

[ApiController]
[Route("api/[controller]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class JobController(IMediator mediator) : ControllerBase
{
    #region Lookups
    [HttpGet("lookups/sectors")]
    [Authorize(Policy = PermissionPolicyProvider.PolicyPrefix + PermissionNames.JobsView)]
    public async Task<IActionResult> GetSectors()
    {
        var result = await mediator.Send(new GetSectorsQuery());
        return result.ToActionResult();
    }
    [HttpGet("lookups/managements")]
    [Authorize(Policy = PermissionPolicyProvider.PolicyPrefix + PermissionNames.JobsView)]
    public async Task<IActionResult> GetManagements([FromQuery] Guid sectorId)
    {
        var result = await mediator.Send(new GetManagementsBySectorQuery(sectorId));
        return result.ToActionResult();
    }

    [HttpGet("lookups/departments")]
    [Authorize(Policy = PermissionPolicyProvider.PolicyPrefix + PermissionNames.JobsView)]
    public async Task<IActionResult> GetDepartments([FromQuery] Guid managementId)
    {
        var result = await mediator.Send(new GetDepartmentsByManagementQuery(managementId));
        return result.ToActionResult();
    }

    [HttpGet("lookups/majors")]
    [Authorize(Policy = PermissionPolicyProvider.PolicyPrefix + PermissionNames.JobsView)]
    public async Task<IActionResult> GetMajors([FromQuery] GetMajorsQuery query)
   {
        var language = Request.Headers.AcceptLanguage.ToString();
        var result = await mediator.Send(query with { Language = language });
        return result.ToActionResult();
    }

    [HttpGet("lookups/sub-majors")]
    [Authorize(Policy = PermissionPolicyProvider.PolicyPrefix + PermissionNames.JobsView)]
    public async Task<IActionResult> GetSubMajors([FromQuery] GetSubMajorsQuery query)
    {
        var result = await mediator.Send(query);
        return result.ToActionResult();
    }

    [HttpGet("lookups/skills")]
    [Authorize(Policy = PermissionPolicyProvider.PolicyPrefix + PermissionNames.JobsView)]
    public async Task<IActionResult> GetSkills([FromQuery] Guid majorId)
    {
        var result = await mediator.Send(new GetSkillByMajorQuery(majorId));
        return result.ToActionResult();
    }

    [HttpGet("lookups/degrees")]
    [Authorize(Policy = PermissionPolicyProvider.PolicyPrefix + PermissionNames.JobsView)]
    public async Task<IActionResult> GetDegrees()
    {
        var result = await mediator.Send(new GetDegreesQuery());
        return result.ToActionResult();
    }

    [HttpGet("lookups/work-types")]
    [Authorize(Policy = PermissionPolicyProvider.PolicyPrefix + PermissionNames.JobsView)]
    public async Task<IActionResult> GetWorkTypes()
    {
        var result = await mediator.Send(new GetWorkTypesQuery());
        return result.ToActionResult();
    }
    
    [HttpGet("lookups/job-categories")]
    [Authorize(Policy = PermissionPolicyProvider.PolicyPrefix + PermissionNames.JobsView)]
    public async Task<IActionResult> GetJobCategories()
    {
        var result = await mediator.Send(new GetJobCategoriesQuery());
        return result.ToActionResult();
    }
    
    [HttpGet("lookups/genders")]
    [Authorize(Policy = PermissionPolicyProvider.PolicyPrefix + PermissionNames.JobsView)]
    public async Task<IActionResult> GetGenders()
    {
        var result = await mediator.Send(new GetGendersWithAllQuery());
        return result.ToActionResult();
    }
    
    [HttpGet("lookups/target-entities")]
    [Authorize(Policy = PermissionPolicyProvider.PolicyPrefix + PermissionNames.JobsView)]
    public async Task<IActionResult> GetTargetEntities()
    {
        var result = await mediator.Send(new GetTargetEntitiesQuery());
        return result.ToActionResult();
    }

    [HttpGet("lookups/nationalities")]
    [Authorize(Policy = PermissionPolicyProvider.PolicyPrefix + PermissionNames.JobsView)]
    public async Task<IActionResult> GetNationalities()
    {
        var language = Request.Headers.AcceptLanguage.ToString();
        var result = await mediator.Send(new GetCountriesQuery() with {Language = language});
        return result.ToActionResult();
    }
    
    [HttpGet("lookups/job-status")]
    [Authorize(Policy = PermissionPolicyProvider.PolicyPrefix + PermissionNames.JobsView)]
    public async Task<IActionResult> GetJobStatus()
    {
        var result = await mediator.Send(new GetJobStatusesQuery());
        return result.ToActionResult();
    }

    [HttpGet("lookups/invitation-statuses")]
    [Authorize(Policy = PermissionPolicyProvider.PolicyPrefix + PermissionNames.JobsView)]
    public async Task<IActionResult> GetInvitationStatuses()
    {
        var result = await mediator.Send(new GetInvitationStatusesQuery());
        return result.ToActionResult();
    }
    #endregion

    #region Job CRUD Operations
    [HttpPost]
    [Authorize(Policy = PermissionPolicyProvider.PolicyPrefix + PermissionNames.JobsManage)]
    public async Task<IActionResult> CreateJob([FromBody] CreateJobCommand command)
    {
        var result = await mediator.Send(command);
        return result.ToActionResult();
    }

     [HttpGet("{id:guid}")]
     [Authorize(Policy = PermissionPolicyProvider.PolicyPrefix + PermissionNames.JobsView)]
     public async Task<IActionResult> GetJob(Guid id)
     {
         var result = await mediator.Send(new GetJobByIdQuery(id));
         return result.ToActionResult();
     }

    [HttpPost("search")]
    [Authorize(Policy = PermissionPolicyProvider.PolicyPrefix + PermissionNames.JobsView)]
    public async Task<IActionResult> GetJobs([FromBody] GetJobsQuery query)
    {
        var result = await mediator.Send(query);
        return result.ToActionResult();
    }

    [HttpPut]
    [Authorize(Policy = PermissionPolicyProvider.PolicyPrefix + PermissionNames.JobsManage)]
    public async Task<IActionResult> UpdateJob([FromBody] UpdateJobCommand command)
    {
        var result = await mediator.Send(command);
        return result.ToActionResult();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = PermissionPolicyProvider.PolicyPrefix + PermissionNames.JobsManage)]
    public async Task<IActionResult> DeleteJob(Guid id)
    {
        var result = await mediator.Send(new DeleteJobCommand(id));
        return result.ToActionResult();
    }

    [HttpPut("{id:guid}/status")]
    [Authorize(Policy = PermissionPolicyProvider.PolicyPrefix + PermissionNames.JobsApprove)]
    public async Task<IActionResult> ChangeJobStatus(
    Guid id,
    [FromQuery] Guid statusId)
    {
        var command = new ChangeJobStatusCommand(id, statusId);
        var result = await mediator.Send(command);
        return result.ToActionResult();
    }
    #endregion

    #region Job Quireies
    [HttpGet("stats/count")]
    [Authorize(Policy = PermissionPolicyProvider.PolicyPrefix + PermissionNames.JobsView)]
    public async Task<IActionResult> GetJobCountByJobStats(
    [FromQuery] Guid jobStatusId)
    {
        var result = await mediator.Send(
            new GetJobCountByJobStatsQuery(jobStatusId));

        return result.ToActionResult();
    }
    #endregion
}
