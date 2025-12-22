using MediatR;
using Microsoft.AspNetCore.Mvc;
using Tawtheef.Application.Features.Operations.Employee.Job.Commands;
using Tawtheef.Application.Features.Operations.Employee.Job.Queries;
using Tawtheef.Infrastructure.Extensions;

namespace Operations.API.Controllers.Employee;

[ApiController]
[Route("api/[controller]")]
public class JobPointsController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> AddJob([FromBody] SaveJobPointsCommand command)
    {
        var result = await mediator.Send(command);
        return result.ToActionResult();
    }

    [HttpGet("{jobId:guid}")]
    public async Task<IActionResult> GetJobTabReviews(Guid jobId)
    {
        var result = await mediator.Send(new GetJobPointsByJobIdQuery(jobId));
        return result.ToActionResult();
    }
}
