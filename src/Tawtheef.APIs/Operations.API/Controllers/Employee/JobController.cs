using Application.Operation.Features.Employee.JobManagement.JobOperations.Commands;
using Application.Operation.Features.Employee.JobManagement.JobOperations.DTOs;
using Application.Operation.Features.Employee.JobManagement.JobOperations.Queries;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Tawtheef.Application.Common.Security;
using Tawtheef.Application.Features.Lookups.Queries;
using Tawtheef.Infrastructure.Extensions;

using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Common.Interfaces.Services.Security;

namespace Operations.API.Controllers.Employee;

[ApiController]
[Route("api/[controller]")]
[Microsoft.AspNetCore.Authorization.Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class JobController(
    IMediator mediator,
    ICurrentUserService currentUserService,
    IJobRepository jobRepository) : ControllerBase
{
    private async Task<bool> HasFullJobAccessOrIsCreator(Guid jobId)
    {
        if (User.HasFullJobAccess()) return true;

        var currentUserIdStr = currentUserService.UserId;
        if (!Guid.TryParse(currentUserIdStr, out var currentUserId)) return false;

        var jobResult = await jobRepository.Repository.GetByIdAsync(jobId);
        if (jobResult.IsFailed || jobResult.Value == null) return false;

        return jobResult.Value.CreatedById == currentUserId;
    }
    #region Lookups
    [HttpGet("lookups/sectors")]
    [AuthorizePermission(PermissionKeys.Jobs.View, PermissionKeys.Jobs.Edit)]
    public async Task<IActionResult> GetSectors()
    {
        var result = await mediator.Send(new GetSectorsQuery());
        return result.ToActionResult();
    }
    [HttpGet("lookups/managements")]
    [AuthorizePermission(PermissionKeys.Jobs.View, PermissionKeys.Jobs.Edit)]
    public async Task<IActionResult> GetManagements([FromQuery] Guid sectorId)
    {
        var result = await mediator.Send(new GetManagementsBySectorQuery(sectorId));
        return result.ToActionResult();
    }

    [HttpGet("lookups/departments")]
    [AuthorizePermission(PermissionKeys.Jobs.View, PermissionKeys.Jobs.Edit)]
    public async Task<IActionResult> GetDepartments([FromQuery] Guid managementId)
    {
        var result = await mediator.Send(new GetDepartmentsByManagementQuery(managementId));
        return result.ToActionResult();
    }

    [HttpGet("lookups/majors")]
    [AuthorizePermission(PermissionKeys.Jobs.View, PermissionKeys.Jobs.Edit)]
    public async Task<IActionResult> GetMajors([FromQuery] GetMainMajorsQuery query)
   {
        var language = Request.Headers.AcceptLanguage.ToString();
        var result = await mediator.Send(query with { Language = language });
        return result.ToActionResult();
    }

    [HttpGet("lookups/sub-majors")]
    [AuthorizePermission(PermissionKeys.Jobs.View, PermissionKeys.Jobs.Edit)]
    public async Task<IActionResult> GetSubMajors([FromQuery] GetSubMajorsQuery query)
    {
        var result = await mediator.Send(query);
        return result.ToActionResult();
    }

    [HttpGet("lookups/skills")]
    [AuthorizePermission(PermissionKeys.Jobs.View, PermissionKeys.Jobs.Edit)]
    public async Task<IActionResult> GetSkills([FromQuery] List<Guid> majorIds)
    {
        var result = await mediator.Send(new GetSkillBySubMajorIdAndRelatedParentSkillQuery(majorIds));
        return result.ToActionResult();
    }

    [HttpGet("lookups/degrees")]
    [AuthorizePermission(PermissionKeys.Jobs.View, PermissionKeys.Jobs.Edit)]
    public async Task<IActionResult> GetDegrees()
    {
        var result = await mediator.Send(new GetDegreesQuery());
        return result.ToActionResult();
    }

    [HttpGet("lookups/work-types")]
    [AuthorizePermission(PermissionKeys.Jobs.View, PermissionKeys.Jobs.Edit)]
    public async Task<IActionResult> GetWorkTypes()
    {
        var result = await mediator.Send(new GetWorkTypesQuery());
        return result.ToActionResult();
    }
    
    [HttpGet("lookups/job-titles")]
    [AuthorizePermission(PermissionKeys.Jobs.View, PermissionKeys.Jobs.Edit)]
    public async Task<IActionResult> GetJobTitles([FromQuery] GetJobTitlesQuery query)
    {
        var language = Request.Headers.AcceptLanguage.ToString();
        var result = await mediator.Send(query with { Language = language });
        return result.ToActionResult();
    }

    [HttpGet("lookups/job-categories")]
    [AuthorizePermission(PermissionKeys.Jobs.View, PermissionKeys.Jobs.Edit)]
    public async Task<IActionResult> GetJobCategories()
    {
        var result = await mediator.Send(new GetJobCategoriesQuery());
        return result.ToActionResult();
    }
    
    [HttpGet("lookups/genders")]
    [AuthorizePermission(PermissionKeys.Jobs.View, PermissionKeys.Jobs.Edit)]
    public async Task<IActionResult> GetGenders()
    {
        var result = await mediator.Send(new GetGendersWithAllQuery());
        return result.ToActionResult();
    }
    
    [HttpGet("lookups/target-entities")]
    [AuthorizePermission(PermissionKeys.Jobs.View, PermissionKeys.Jobs.Edit)]
    public async Task<IActionResult> GetTargetEntities()
    {
        var result = await mediator.Send(new GetTargetEntitiesQuery());
        return result.ToActionResult();
    }

    [HttpGet("lookups/nationalities")]
    [AuthorizePermission(PermissionKeys.Jobs.View, PermissionKeys.Jobs.Edit)]
    public async Task<IActionResult> GetNationalities()
    {
        var language = Request.Headers.AcceptLanguage.ToString();
        var result = await mediator.Send(new GetCountriesQuery() with {Language = language});
        return result.ToActionResult();
    }
    
    [HttpGet("lookups/job-status")]
    [AuthorizePermission(PermissionKeys.Jobs.View, PermissionKeys.Jobs.Edit)]
    public async Task<IActionResult> GetJobStatus()
    {
        var result = await mediator.Send(new GetJobStatusesQuery());
        return result.ToActionResult();
    }

    [HttpGet("lookups/invitation-statuses")]
    [AuthorizePermission(PermissionKeys.Jobs.View, PermissionKeys.Jobs.Edit)]
    public async Task<IActionResult> GetInvitationStatuses()
    {
        var result = await mediator.Send(new GetInvitationStatusesQuery());
        return result.ToActionResult();
    }
    #endregion

    #region Job CRUD Operations
    [HttpPost]
    [AuthorizePermission(PermissionKeys.Jobs.Create)]
    public async Task<IActionResult> CreateJob([FromBody] CreateJobCommand command)
    {
        var result = await mediator.Send(command);
        return result.ToActionResult();
    }

    [HttpGet("{id:guid}")]
    [AuthorizePermission(PermissionKeys.Jobs.View, PermissionKeys.Jobs.Edit)]
    public async Task<IActionResult> GetJob(Guid id)
    {
        var result = await mediator.Send(new GetJobByIdQuery(id));
        return result.ToActionResult();
    }
     
     [HttpPost("{id:guid}/copy")]
     [AuthorizePermission(PermissionKeys.Jobs.Clone)]
     public async Task<IActionResult> CreateJobFromPrevious(
         Guid id,
         [FromBody] CreateJobFromPreviousDto job)
     {
         var result = await mediator.Send(new CreateJobFromPreviousCommand(id, job));
         return result.ToActionResult();
     }

     [HttpGet("{id:guid}/copy-template")]
     [AuthorizePermission(PermissionKeys.Jobs.View, PermissionKeys.Jobs.Edit)]
     public async Task<IActionResult> GetJobCopyTemplate(Guid id)
     {
         var result = await mediator.Send(new GetJobCopyTemplateQuery(id));
         return result.ToActionResult();
     }


    [HttpPost("search")]
    [AuthorizePermission(PermissionKeys.Jobs.View, PermissionKeys.Jobs.Edit)]
    public async Task<IActionResult> GetJobs([FromBody] GetJobsQuery query)
    {
        var result = await mediator.Send(query);
        return result.ToActionResult();
    }

    [HttpPost("export")]
    [AuthorizePermission(PermissionKeys.Dashboard.Export)]
    public async Task<IActionResult> ExportJobs([FromBody] ExportJobsQuery query)
    {
        var result = await mediator.Send(query);
        return result.IsFailed
            ? result.ToActionResult()
            : File(result.Value.Content, result.Value.ContentType, result.Value.FileName);
    }

    [HttpDelete("{id:guid}")]
    [AuthorizePermission(PermissionKeys.Jobs.Delete)]
    public async Task<IActionResult> DeleteJob(Guid id)
    {
        if (!await HasFullJobAccessOrIsCreator(id)) return Forbid();
        var result = await mediator.Send(new DeleteJobCommand(id));
        return result.ToActionResult();
    }

    [HttpDelete("specialization/{specializationId:guid}")]
    [AuthorizePermission(PermissionKeys.Jobs.Edit)]
    public async Task<IActionResult> DeleteJobSpecialization(Guid specializationId)
    {
        var result = await mediator.Send(new DeleteJobSpecializationCommand(specializationId));
        return result.ToActionResult();
    }

    [HttpPut("{id:guid}/status")]
    [AuthorizePermission(
        PermissionKeys.Jobs.Edit, 
        PermissionKeys.Jobs.Delete,
        PermissionKeys.Jobs.Approve,
        PermissionKeys.Jobs.Cancel,
        PermissionKeys.JobsPoints.Edit,
        PermissionKeys.JobsPoints.Approve,
        PermissionKeys.Jobs.Publish,
        PermissionKeys.Jobs.Clone)]
    public async Task<IActionResult> ChangeJobStatus(
    Guid id,
    [FromQuery] Guid statusId)
    {
        var command = new ChangeJobStatusCommand(id, statusId);
        var result = await mediator.Send(command);
        return result.ToActionResult();
    }

    [HttpGet("check-duplication")]
    [AuthorizePermission(PermissionKeys.Jobs.Edit)]
    public async Task<IActionResult> CheckDuplication([FromQuery] CheckDuplicationQuery query)
    {
        var result = await mediator.Send(query);
        return result.ToActionResult();
    }

    [HttpGet("{id:guid}/has-invitations")]
    [AuthorizePermission(PermissionKeys.Jobs.Cancel)]
    public async Task<IActionResult> CheckJobInvitations(Guid id)
    {
        var result = await mediator.Send(new CheckJobInvitationsQuery(id));
        return result.ToActionResult();
    }
    #endregion

    #region Job Section Updates

    [HttpPut("{id:guid}/basics")]
    [AuthorizePermission(PermissionKeys.Jobs.Edit)]
    public async Task<IActionResult> UpdateJobBasics(Guid id, [FromBody] UpdateJobBasicsDto dto)
    {
        if (!await HasFullJobAccessOrIsCreator(id)) return Forbid();
        var result = await mediator.Send(new UpdateJobBasicsCommand(id, dto));
        return result.ToActionResult();
    }

    [HttpPut("{id:guid}/overview")]
    [AuthorizePermission(PermissionKeys.Jobs.Edit)]
    public async Task<IActionResult> UpdateJobOverview(Guid id, [FromBody] UpdateJobOverviewDto dto)
    {
        if (!await HasFullJobAccessOrIsCreator(id)) return Forbid();
        var result = await mediator.Send(new UpdateJobOverviewCommand(id, dto));
        return result.ToActionResult();
    }

    [HttpPut("{id:guid}/qualifications")]
    [AuthorizePermission(PermissionKeys.Jobs.Edit)]
    public async Task<IActionResult> UpdateJobQualifications(Guid id, [FromBody] UpdateJobQualificationsDto dto)
    {
        if (!await HasFullJobAccessOrIsCreator(id)) return Forbid();
        var result = await mediator.Send(new UpdateJobQualificationsCommand(id, dto));
        return result.ToActionResult();
    }

    [HttpPut("{id:guid}/responsibilities")]
    [AuthorizePermission(PermissionKeys.Jobs.Edit)]
    public async Task<IActionResult> UpdateJobResponsibilities(Guid id, [FromBody] UpdateJobResponsibilitiesDto dto)
    {
        if (!await HasFullJobAccessOrIsCreator(id)) return Forbid();
        var result = await mediator.Send(new UpdateJobResponsibilitiesCommand(id, dto));
        return result.ToActionResult();
    }

    [HttpPut("{id:guid}/conditions")]
    [AuthorizePermission(PermissionKeys.Jobs.Edit)]
    public async Task<IActionResult> UpdateJobConditions(Guid id, [FromBody] UpdateJobConditionsDto dto)
    {
        if (!await HasFullJobAccessOrIsCreator(id)) return Forbid();
        var result = await mediator.Send(new UpdateJobConditionsCommand(id, dto));
        return result.ToActionResult();
    }

    [HttpPut("{id:guid}/skills")]
    [AuthorizePermission(PermissionKeys.Jobs.Edit)]
    public async Task<IActionResult> UpdateJobSkills(Guid id, [FromBody] UpdateJobSkillsDto dto)
    {
        if (!await HasFullJobAccessOrIsCreator(id)) return Forbid();
        var result = await mediator.Send(new UpdateJobSkillsCommand(id, dto));
        return result.ToActionResult();
    }

    [HttpPut("{id:guid}/attachments")]
    [AuthorizePermission(PermissionKeys.Jobs.Edit)]
    public async Task<IActionResult> UpdateJobAttachments(Guid id, [FromBody] UpdateJobAttachmentsDto dto)
    {
        if (!await HasFullJobAccessOrIsCreator(id)) return Forbid();
        var result = await mediator.Send(new UpdateJobAttachmentsCommand(id, dto));
        return result.ToActionResult();
    }

    [HttpPut("{id:guid}/benefits")]
    [AuthorizePermission(PermissionKeys.Jobs.Edit)]
    public async Task<IActionResult> UpdateJobBenefits(Guid id, [FromBody] UpdateJobBenefitsDto dto)
    {
        if (!await HasFullJobAccessOrIsCreator(id)) return Forbid();
        var result = await mediator.Send(new UpdateJobBenefitsCommand(id, dto));
        return result.ToActionResult();
    }

    #endregion

    #region Job Quireies
    [HttpGet("stats/count")]
    [AuthorizePermission(PermissionKeys.Jobs.View, PermissionKeys.Jobs.Edit)]
    public async Task<IActionResult> GetJobCountByJobStats(
    [FromQuery] Guid jobStatusId)
    {
        var result = await mediator.Send(
            new GetJobCountByJobStatsQuery(jobStatusId));

        return result.ToActionResult();
    }
    #endregion
}

