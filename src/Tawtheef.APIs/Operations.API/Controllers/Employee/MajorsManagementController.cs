using Application.Operation.Features.Employee.ManagementMajorSkill.Majors.Commands;
using Application.Operation.Features.Employee.ManagementMajorSkill.Majors.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Tawtheef.Infrastructure.Extensions;

namespace Operations.API.Controllers.Employee;

[ApiController]
[Route("api/[controller]")]
public class MajorsManagementController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [Route("GetMainMajors")]
    public async Task<IActionResult> GetMainMajors([FromQuery] GetMainMajorsQuery query,CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query,cancellationToken);
        return result.ToActionResult();
    }
    [HttpGet]
    [Route("GetSubMajors")]
    public async Task<IActionResult> GetSubMajors([FromQuery] GetSubMajorsQuery query,CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query,cancellationToken);
        return result.ToActionResult();
    }
    [HttpPost]
    public async Task<IActionResult> CreateMajor([FromBody] CreateMajorCommand command)
    {
        var result = await mediator.Send(command);
        return result.ToActionResult();
    }

    [HttpPut]
    public async Task<IActionResult> UpdateMajor([FromBody] UpdateMajorCommand command)
    {
        var result = await mediator.Send(command);
        return result.ToActionResult();
    }

    [HttpPut]
    [Route("change-activation")]
    public async Task<IActionResult> ChangeActiveStatusMajor([FromBody] ChangeActiveStatusMajorCommand command)
    {
        var result = await mediator.Send(command);
        return result.ToActionResult();
    }
}

