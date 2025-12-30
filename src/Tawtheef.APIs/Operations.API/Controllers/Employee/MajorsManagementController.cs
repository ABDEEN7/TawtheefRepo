using MediatR;
using Microsoft.AspNetCore.Mvc;
using Tawtheef.Application.Features.Lookups.Queries;
using Tawtheef.Application.Features.Operations.Employee.ManagementMajorSkill.Majors.Commands;
using Tawtheef.Infrastructure.Extensions;

namespace Operations.API.Controllers.Employee;

[ApiController]
[Route("api/[controller]")]
public class MajorsManagementController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllMajors(GetMajorsQuery query,CancellationToken cancellationToken)
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
