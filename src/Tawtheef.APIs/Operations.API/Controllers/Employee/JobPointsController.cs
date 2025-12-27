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

    #region Points Configurations

    [HttpGet("{jobId:guid}/config")]
    public async Task<IActionResult> GetJobPointsConfigurations(Guid jobId)
    {
        var result = await mediator.Send(new GetJobPointsConfigurationsByJobIdQuery(jobId));
        return result.ToActionResult();
    }

    [HttpPost("configurations/add")]
    public async Task<IActionResult> AddJobPointsConfiguration([FromBody] AddJobPointsConfigurationCommand command)
    {
        var result = await mediator.Send(command);
        return result.ToActionResult();
    }
    #endregion
}
