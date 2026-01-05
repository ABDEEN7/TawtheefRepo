
using Cortex.Mediator;
using Microsoft.AspNetCore.Mvc;
using Tawtheef.Application.Common;
using Tawtheef.Application.Features.Operations.Employee.Job.Commands;
using Tawtheef.Application.Features.Operations.Employee.Job.Queries;
using Tawtheef.Infrastructure.Extensions;

namespace Operations.API.Controllers.Employee;

[ApiController]
[Route("api/[controller]")]
public class JobApprovalController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> AddReviewJob([FromForm] SaveJobReviewCommand command)
    {
        var result = await mediator.Send(command);
        return result.ToActionResult();
    }

    [HttpGet("{jobId:guid}")]
    public async Task<IActionResult> GetJobTabReviews(Guid jobId)
    {
        var result = await mediator.Send(new GetJobTabReviewsQuery(jobId));
            return result.ToActionResult();
    }

    [HttpGet("{jobId:guid}/latest")]
    public async Task<IActionResult> GetLatestJobTabReviews(Guid jobId)
    {
        var result = await mediator.Send(new GetLatestReviewQuery(jobId));
        return result.ToActionResult();
    }
}
