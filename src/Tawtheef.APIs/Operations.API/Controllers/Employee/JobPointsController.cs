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
    public async Task<IActionResult> AddJobPoints([FromBody] SaveJobPointsCommand command)
    {
        var result = await mediator.Send(command);
        return result.ToActionResult();
    }

    [HttpGet("{jobId:guid}")]
    public async Task<IActionResult> GetJobPoints(Guid jobId)
    {
        var result = await mediator.Send(new GetJobPointsByJobIdQuery(jobId));
        return result.ToActionResult();
    }

    [HttpPost("{jobId:guid}/approve")]
    public async Task<IActionResult> ApproveJobPoints(Guid jobId)
    {
        var result = await mediator.Send(new ApproveJobPointsCommand(jobId));
        return result.ToActionResult();
    }

    #region Points Configurations

    [HttpGet("config")]
    public async Task<IActionResult> GetJobPointsConfigurations()
    {
        var result = await mediator.Send(new GetJobPointsConfigurationsQuery());
        return result.ToActionResult();
    }

    [HttpPost("configurations/save")]
    public async Task<IActionResult> SaveJobPointsConfiguration([FromBody] SaveJobPointsConfigurationCommand command)
    {
        var result = await mediator.Send(command);
        return result.ToActionResult();
    }
    #endregion
}
