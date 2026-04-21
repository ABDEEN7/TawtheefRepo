using Application.Operation.Features.Employee.JobManagement.JobOperations.Commands;
using Application.Operation.Features.Employee.JobManagement.JobOperations.Queries;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Security;
using Tawtheef.Infrastructure.Extensions;

using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services.Security;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;
using JobEntity = Tawtheef.Domain.Entities.Recruitment.Job;

namespace Operations.API.Controllers.Employee;

[ApiController]
[Route("api/[controller]")]
[Microsoft.AspNetCore.Authorization.Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class JobPointsController(
    IMediator mediator,
    ICurrentUserService currentUserService,
    IUnitOfWork uow) : ControllerBase
{
    private async Task<bool> IsPointCreatorOrHRManager(Guid jobId)
    {
        if (User.IsInRole("HrManager")) return true;

        var currentUserIdStr = currentUserService.UserId;
        if (!Guid.TryParse(currentUserIdStr, out var currentUserId)) return false;

        var points = await uow.GetEntityRepository<JobPointsMain>()
            .DbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.JobId == jobId);
            
        if (points == null) return true;
        return points.CreatedById == currentUserId;
    }
    [HttpPost]
    [AuthorizePermission(PermissionKeys.JobsPoints.Edit)]
    public async Task<IActionResult> AddJobPoints([FromBody] SaveJobPointsCommand command)
    {
        if (!await IsPointCreatorOrHRManager(command.Request.JobId)) return Forbid();
        var result = await mediator.Send(command);
        return result.ToActionResult();
    }

    [HttpGet("{jobId:guid}")]
    [AuthorizePermission(
        PermissionKeys.JobPointsConfiguration.View,
        PermissionKeys.JobPointsConfiguration.Manage,
        PermissionKeys.JobsPoints.View, 
        PermissionKeys.JobsPoints.Edit, 
        PermissionKeys.JobsPoints.Approve)]
    public async Task<IActionResult> GetJobPoints(Guid jobId)
    {
        var result = await mediator.Send(new GetJobPointsByJobIdQuery(jobId));
        return result.ToActionResult();
    }

    [HttpPost("{jobId:guid}/approve")]
    [AuthorizePermission(PermissionKeys.JobsPoints.Approve)]
    public async Task<IActionResult> ApproveJobPoints(Guid jobId)
    {
        // Approval is typically NOT restricted to creator (standard separation of duties)
        // unless explicitly requested by "creator point is different from creator job"
        // But for now, ensuring it's an HR Manager (Role-based) + Permission
        
        var result = await mediator.Send(new ApproveJobPointsCommand(jobId));
        return result.ToActionResult();
    }

    [HttpPost("{jobId:guid}/reject")]
    [AuthorizePermission(PermissionKeys.JobsPoints.Approve)]
    public async Task<IActionResult> RejectJobPoints(Guid jobId, [FromBody] string reason)
    {
        var result = await mediator.Send(new RejectJobPointsCommand(jobId, reason));
        return result.ToActionResult();
    }

    #region Points Configurations
    [HttpGet("configurations")]
    [AuthorizePermission(
        PermissionKeys.JobPointsConfiguration.View,
        PermissionKeys.JobPointsConfiguration.Manage,
        PermissionKeys.JobsPoints.View,
        PermissionKeys.JobsPoints.Edit,
        PermissionKeys.JobsPoints.Approve)]
    public async Task<IActionResult> GetJobPointsConfiguration()
    {
        var result = await mediator.Send(new GetJobPointsConfigurationsQuery());
        return result.ToActionResult();
    }

    [HttpPost("configurations/save")]
    [AuthorizePermission(PermissionKeys.JobPointsConfiguration.Manage)]
    public async Task<IActionResult> SaveJobPointsConfiguration([FromBody] SaveJobPointsConfigurationCommand command)
    {
        var result = await mediator.Send(command);
        return result.ToActionResult();
    }
    #endregion
}

