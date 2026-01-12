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
public class JobApprovalController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [AuthorizePermission(PermissionKeys.Jobs.Manage)]
    public async Task<IActionResult> AddReviewJob([FromForm] SaveJobReviewCommand command)
    {
        var result = await mediator.Send(command);
        return result.ToActionResult();
    }

    [HttpPut]
    [AuthorizePermission(PermissionKeys.Jobs.Manage)]
    public async Task<IActionResult> UpdateReviewJob([FromForm] UpdateJobReviewCommand command)
    {
        var result = await mediator.Send(command);
        return result.ToActionResult();
    }

    [HttpGet("{jobId:guid}")]
    [AuthorizePermission(PermissionKeys.Jobs.View, PermissionKeys.Jobs.Manage)]
    public async Task<IActionResult> GetJobTabReviews(Guid jobId)
    {
        var result = await mediator.Send(new GetJobTabReviewsQuery(jobId));
        return result.ToActionResult();
    }

    [HttpGet("{jobId:guid}/latest")]
    [AuthorizePermission(PermissionKeys.Jobs.View, PermissionKeys.Jobs.Manage)]
    public async Task<IActionResult> GetLatestJobTabReviews(Guid jobId)
    {
        var result = await mediator.Send(new GetLatestReviewQuery(jobId));
        return result.ToActionResult();
    }
}
