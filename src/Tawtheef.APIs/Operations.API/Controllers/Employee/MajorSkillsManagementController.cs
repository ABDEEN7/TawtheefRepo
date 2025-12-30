using MediatR;
using Microsoft.AspNetCore.Mvc;
using Tawtheef.Application.Features.Operations.Employee.ManagementMajorSkill.Mapping.Commands;
using Tawtheef.Application.Features.Operations.Employee.ManagementMajorSkill.Mapping.Queries;
using Tawtheef.Infrastructure.Extensions;

namespace Operations.API.Controllers.Employee;

[ApiController]
[Route("api/[controller]")]
public class MajorSkillsManagementController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllMajorSkills(GetMajorSkillsQuery query, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query, cancellationToken);
        return result.ToActionResult();
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetMajorSkillById([FromRoute] GetMajorSkillByIdQuery query, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query, cancellationToken);
        return result.ToActionResult();
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateMajorSkill([FromBody] CreateMajorSkillCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return result.ToActionResult();
    }
    
    [HttpPut]
    public async Task<IActionResult> UpdateMajorSkill([FromBody] UpdateMajorSkillCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPut]
    [Route("change-activation")]
    public async Task<IActionResult> ChangeMajorSkillActivation([FromBody] ChangeActiveStatusMajorSkillCommand command,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return result.ToActionResult();
    }
}
