using MediatR;
using Microsoft.AspNetCore.Mvc;
using Tawtheef.Application.Features.Lookups.Queries;
using Tawtheef.Application.Features.Operations.Employee.Job.Commands;
using Tawtheef.Application.Features.Operations.Employee.Job.Queries;
using Tawtheef.Infrastructure.Extensions;

namespace Operations.API.Controllers.Employee;

[ApiController]
[Route("api/[controller]")]
//[Authorize(Policy = PermissionPolicyProvider.POLICY_PREFIX + PermissionNames.JobsManage)]
public class JobController(IMediator mediator) : ControllerBase
{
    #region Lookups
    [HttpGet("lookups/departments")]
    public async Task<IActionResult> GetDepartments()
    {
        var result = await mediator.Send(new GetDepartmentsQuery());
        return result.ToActionResult();
    }
    
    [HttpGet("lookups/majors")]
    public async Task<IActionResult> GetMajors()
    {
        var result = await mediator.Send(new GetMajorsQuery());
        return result.ToActionResult();
    }
    
    [HttpGet("lookups/degrees")]
    public async Task<IActionResult> GetDegrees()
    {
        var result = await mediator.Send(new GetDegreesQuery());
        return result.ToActionResult();
    }

    [HttpGet("lookups/work-types")]
    public async Task<IActionResult> GetWorkTypes()
    {
        var result = await mediator.Send(new GetWorkTypesQuery());
        return result.ToActionResult();
    }
    
    [HttpGet("lookups/job-categories")]
    public async Task<IActionResult> GetJobCategories()
    {
        var result = await mediator.Send(new GetJobCategoriesQuery());
        return result.ToActionResult();
    }
    
    [HttpGet("lookups/genders")]
    public async Task<IActionResult> GetGenders()
    {
        var result = await mediator.Send(new GetGendersQuery());
        return result.ToActionResult();
    }
    
    [HttpGet("lookups/target-entities")]
    public async Task<IActionResult> GetTargetEntities()
    {
        var result = await mediator.Send(new GetTargetEntitiesQuery());
        return result.ToActionResult();
    }

    [HttpGet("lookups/nationalities")]
    public async Task<IActionResult> GetNationalities()
    {
        var result = await mediator.Send(new GetCountriesQuery());
        return result.ToActionResult();
    }
    
    [HttpGet("lookups/job-status")]
    public async Task<IActionResult> GetJobStatus()
    {
        var result = await mediator.Send(new GetJobStatusesQuery());
        return result.ToActionResult();
    }

    [HttpGet("lookups/invitation-statuses")]
    public async Task<IActionResult> GetInvitationStatuses()
    {
        var result = await mediator.Send(new GetInvitationStatusesQuery());
        return result.ToActionResult();
    }
    #endregion

    #region Job CRUD Operations
    [HttpPost]
    public async Task<IActionResult> CreateJob([FromBody] CreateJobCommand command)
    {
        var result = await mediator.Send(command);
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }
    
        return BadRequest(result.IsFailed);
    }

     [HttpGet("{id:guid}")]
     public async Task<IActionResult> GetJob(Guid id)
     {
         var result = await mediator.Send(new GetJobByIdQuery(id));
         return result.ToActionResult();
     }

    [HttpGet]
    public async Task<IActionResult> GetJobs([FromQuery] GetJobsQuery query)
    {
        var result = await mediator.Send(query);
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }
    
        return BadRequest(result.IsFailed);
    }
    

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateJob(Guid id, [FromBody] UpdateJobCommand command)
    {
        // Ensure the ID in the route matches the command
        if (id != command.JobId)
        {
            return BadRequest("Route ID does not match command ID");
        }
        
        var result = await mediator.Send(command);
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }
    
        return BadRequest(result.IsFailed);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteJob(Guid id)
    {
        var result = await mediator.Send(new DeleteJobCommand(id));
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }
    
        return BadRequest(result.IsFailed);
    }
    #endregion
}
