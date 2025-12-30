using MediatR;
using Microsoft.AspNetCore.Mvc;
using Tawtheef.Application.Features.Operations.Employee.ManagementMajorSkill.Skills.Commands;
using Tawtheef.Application.Features.Operations.Employee.ManagementMajorSkill.Skills.Queries;
using Tawtheef.Infrastructure.Extensions;

namespace Operations.API.Controllers.Employee;

[ApiController]
[Route("api/[controller]")]
public class SkillsManagementController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllSkills(GetSkillsQuery query,CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query,cancellationToken);
        return result.ToActionResult();
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateSkill([FromBody] CreateSkillCommand command,CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command,cancellationToken);
        return result.ToActionResult();
    }
    
    [HttpPut]
    public async Task<IActionResult> UpdateSkill([FromBody] UpdateSkillCommand command,CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command,cancellationToken);
        return result.ToActionResult();
    }
    
    [HttpPut]
    [Route("change-activation")]
    public async Task<IActionResult> ChangeActiveStatusSkill([FromBody] ChangeActiveStatusSkillCommand command,CancellationToken cancellationToken)
    {  
        var result = await mediator.Send(command,cancellationToken);
        return result.ToActionResult();
    }
}
