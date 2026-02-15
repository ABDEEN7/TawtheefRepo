using Application.Operation.Features.Employee.JobManagement.Job.Commands;
using Application.Operation.Features.Employee.JobManagement.Job.DTOs;
using Application.Operation.Features.Employee.JobManagement.Job.Queries;
using Cortex.Mediator;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Tawtheef.Application.Common;
using Tawtheef.Application.Common.Security;
using Tawtheef.Application.Features.Lookups.Queries;
using Tawtheef.Infrastructure.Extensions;

namespace Operations.API.Controllers.Employee;

[ApiController]
[Route("api/[controller]")]
[Microsoft.AspNetCore.Authorization.Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class JobController(IMediator mediator) : ControllerBase
{
    #region Lookups
    [HttpGet("lookups/sectors")]
    [AuthorizePermission(PermissionKeys.Jobs.View, PermissionKeys.Jobs.Manage)]
    public async Task<IActionResult> GetSectors()
    {
        var result = await mediator.Send(new GetSectorsQuery());
        return result.ToActionResult();
    }
    [HttpGet("lookups/managements")]
    [AuthorizePermission(PermissionKeys.Jobs.View, PermissionKeys.Jobs.Manage)]
    public async Task<IActionResult> GetManagements([FromQuery] Guid sectorId)
    {
        var result = await mediator.Send(new GetManagementsBySectorQuery(sectorId));
        return result.ToActionResult();
    }

    [HttpGet("lookups/departments")]
    [AuthorizePermission(PermissionKeys.Jobs.View, PermissionKeys.Jobs.Manage)]
    public async Task<IActionResult> GetDepartments([FromQuery] Guid managementId)
    {
        var result = await mediator.Send(new GetDepartmentsByManagementQuery(managementId));
        return result.ToActionResult();
    }

    [HttpGet("lookups/majors")]
    [AuthorizePermission(PermissionKeys.Jobs.View, PermissionKeys.Jobs.Manage)]
    public async Task<IActionResult> GetMajors([FromQuery] GetMajorsQuery query)
   {
        var language = Request.Headers.AcceptLanguage.ToString();
        var result = await mediator.Send(query with { Language = language });
        return result.ToActionResult();
    }

    [HttpGet("lookups/sub-majors")]
    [AuthorizePermission(PermissionKeys.Jobs.View, PermissionKeys.Jobs.Manage)]
    public async Task<IActionResult> GetSubMajors([FromQuery] GetSubMajorsQuery query)
    {
        var result = await mediator.Send(query);
        return result.ToActionResult();
    }

    [HttpGet("lookups/skills")]
    [AuthorizePermission(PermissionKeys.Jobs.View, PermissionKeys.Jobs.Manage)]
    public async Task<IActionResult> GetSkills([FromQuery] Guid majorId)
    {
        var result = await mediator.Send(new GetSkillByMajorQuery(majorId));
        return result.ToActionResult();
    }

    [HttpGet("lookups/degrees")]
    [AuthorizePermission(PermissionKeys.Jobs.View, PermissionKeys.Jobs.Manage)]
    public async Task<IActionResult> GetDegrees()
    {
        var result = await mediator.Send(new GetDegreesQuery());
        return result.ToActionResult();
    }

    [HttpGet("lookups/work-types")]
    [AuthorizePermission(PermissionKeys.Jobs.View, PermissionKeys.Jobs.Manage)]
    public async Task<IActionResult> GetWorkTypes()
    {
        var result = await mediator.Send(new GetWorkTypesQuery());
        return result.ToActionResult();
    }
    
    [HttpGet("lookups/job-titles")]
    [AuthorizePermission(PermissionKeys.Jobs.View, PermissionKeys.Jobs.Manage)]
    public async Task<IActionResult> GetJobTitles()
    {
        var result = await mediator.Send(new GetJobTitlesQuery());
        return result.ToActionResult();
    }

    [HttpGet("lookups/job-categories")]
    [AuthorizePermission(PermissionKeys.Jobs.View, PermissionKeys.Jobs.Manage)]
    public async Task<IActionResult> GetJobCategories()
    {
        var result = await mediator.Send(new GetJobCategoriesQuery());
        return result.ToActionResult();
    }
    
    [HttpGet("lookups/genders")]
    [AuthorizePermission(PermissionKeys.Jobs.View, PermissionKeys.Jobs.Manage)]
    public async Task<IActionResult> GetGenders()
    {
        var result = await mediator.Send(new GetGendersWithAllQuery());
        return result.ToActionResult();
    }
    
    [HttpGet("lookups/target-entities")]
    [AuthorizePermission(PermissionKeys.Jobs.View, PermissionKeys.Jobs.Manage)]
    public async Task<IActionResult> GetTargetEntities()
    {
        var result = await mediator.Send(new GetTargetEntitiesQuery());
        return result.ToActionResult();
    }

    [HttpGet("lookups/nationalities")]
    [AuthorizePermission(PermissionKeys.Jobs.View, PermissionKeys.Jobs.Manage)]
    public async Task<IActionResult> GetNationalities()
    {
        var language = Request.Headers.AcceptLanguage.ToString();
        var result = await mediator.Send(new GetCountriesQuery() with {Language = language});
        return result.ToActionResult();
    }
    
    [HttpGet("lookups/job-status")]
    [AuthorizePermission(PermissionKeys.Jobs.View, PermissionKeys.Jobs.Manage)]
    public async Task<IActionResult> GetJobStatus()
    {
        var result = await mediator.Send(new GetJobStatusesQuery());
        return result.ToActionResult();
    }

    [HttpGet("lookups/invitation-statuses")]
    [AuthorizePermission(PermissionKeys.Jobs.View, PermissionKeys.Jobs.Manage)]
    public async Task<IActionResult> GetInvitationStatuses()
    {
        var result = await mediator.Send(new GetInvitationStatusesQuery());
        return result.ToActionResult();
    }
    #endregion

    #region Job CRUD Operations
    [HttpPost]
    [AuthorizePermission(PermissionKeys.Jobs.Manage)]
    public async Task<IActionResult> CreateJob([FromBody] CreateJobCommand command)
    {
        var result = await mediator.Send(command);
        return result.ToActionResult();
    }

    [HttpGet("{id:guid}")]
    [AuthorizePermission(PermissionKeys.Jobs.View, PermissionKeys.Jobs.Manage)]
    public async Task<IActionResult> GetJob(Guid id)
    {
        var result = await mediator.Send(new GetJobByIdQuery(id));
        return result.ToActionResult();
    }
     
     [HttpPost("{id:guid}/copy")]
     [AuthorizePermission(PermissionKeys.Jobs.Manage)]
     public async Task<IActionResult> CreateJobFromPrevious(
         Guid id,
         [FromBody] CreateJobFromPreviousDto job)
     {
         var result = await mediator.Send(new CreateJobFromPreviousCommand(id, job));
         return result.ToActionResult();
     }

     [HttpGet("{id:guid}/copy-template")]
     [AuthorizePermission(PermissionKeys.Jobs.View)]
     public async Task<IActionResult> GetJobCopyTemplate(Guid id)
     {
         var result = await mediator.Send(new GetJobCopyTemplateQuery(id));
         return result.ToActionResult();
     }


    [HttpPost("search")]
    [AuthorizePermission(PermissionKeys.Jobs.View, PermissionKeys.Jobs.Manage)]
    public async Task<IActionResult> GetJobs([FromBody] GetJobsQuery query)
    {
        var result = await mediator.Send(query);
        return result.ToActionResult();
    }

    [HttpPut]
    [AuthorizePermission(PermissionKeys.Jobs.Manage)]
    public async Task<IActionResult> UpdateJob([FromBody] UpdateJobCommand command)
    {
        var result = await mediator.Send(command);
        return result.ToActionResult();
    }

    [HttpDelete("{id:guid}")]
    [AuthorizePermission(PermissionKeys.Jobs.Manage)]
    public async Task<IActionResult> DeleteJob(Guid id)
    {
        var result = await mediator.Send(new DeleteJobCommand(id));
        return result.ToActionResult();
    }

    [HttpPut("{id:guid}/status")]
    [AuthorizePermission(PermissionKeys.Jobs.Approve)]
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
    [AuthorizePermission(PermissionKeys.Jobs.View, PermissionKeys.Jobs.Manage)]
    public async Task<IActionResult> GetJobCountByJobStats(
    [FromQuery] Guid jobStatusId)
    {
        var result = await mediator.Send(
            new GetJobCountByJobStatsQuery(jobStatusId));

        return result.ToActionResult();
    }
    #endregion
}
