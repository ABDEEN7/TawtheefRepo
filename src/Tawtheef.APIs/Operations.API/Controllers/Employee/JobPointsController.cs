using Application.Operation.Features.Employee.JobManagement.Job.Commands;
using Application.Operation.Features.Employee.JobManagement.Job.Queries;
using Cortex.Mediator;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Tawtheef.Application.Common;
using Tawtheef.Application.Common.Security;
using Tawtheef.Infrastructure.Extensions;

namespace Operations.API.Controllers.Employee;

[ApiController]
[Route("api/[controller]")]
[Microsoft.AspNetCore.Authorization.Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class JobPointsController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [AuthorizePermission(PermissionKeys.JobsPoints.Manage)]
    public async Task<IActionResult> AddJobPoints([FromBody] SaveJobPointsCommand command)
    {
        var result = await mediator.Send(command);
        return result.ToActionResult();
    }

    [HttpGet("{jobId:guid}")]
    [AuthorizePermission(PermissionKeys.JobsPoints.View, PermissionKeys.JobsPoints.Manage)]
    public async Task<IActionResult> GetJobPoints(Guid jobId)
    {
        var result = await mediator.Send(new GetJobPointsByJobIdQuery(jobId));
        return result.ToActionResult();
    }

    [HttpPost("{jobId:guid}/approve")]
    [AuthorizePermission(PermissionKeys.JobsPoints.Approve)]
    public async Task<IActionResult> ApproveJobPoints(Guid jobId)
    {
        var result = await mediator.Send(new ApproveJobPointsCommand(jobId));
        return result.ToActionResult();
    }

    #region Points Configurations
    [HttpGet("configurations")]
    [AuthorizePermission(PermissionKeys.JobsPoints.View, PermissionKeys.JobsPoints.Manage)]
    public async Task<IActionResult> GetJobPointsConfiguration()
    {
        var result = await mediator.Send(new GetJobPointsConfigurationsQuery());
        return result.ToActionResult();
    }

    [HttpPost("configurations/save")]
    [AuthorizePermission(PermissionKeys.JobsPoints.Manage)]
    public async Task<IActionResult> SaveJobPointsConfiguration([FromBody] SaveJobPointsConfigurationCommand command)
    {
        var result = await mediator.Send(command);
        return result.ToActionResult();
    }
    #endregion
}
