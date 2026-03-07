using Application.Operation.Features.Employee.ManagementMajorSkill.Skills.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Tawtheef.Application.Common;
using Tawtheef.Application.Features.Lookups.Queries;
using Tawtheef.Infrastructure.Extensions;
using GetSkillsQuery = Application.Operation.Features.Employee.ManagementMajorSkill.Skills.Queries.GetSkillsQuery;

namespace Operations.API.Controllers.Employee;

[ApiController]
[Route("api/[controller]")]
public class SkillsManagementController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllSkills([FromQuery] GetSkillsQuery query,CancellationToken cancellationToken)
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
    
    [HttpGet]
    [Route("lookups/skill-types")]
    public async Task<IActionResult> GetSkillTypes([FromQuery] GetSkillTypesQuery query, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query, cancellationToken);
        return result.ToActionResult();
    }
    
    [HttpGet]
    [Route("lookups/skills")]
    public async Task<IActionResult> GetSkills([FromQuery] Tawtheef.Application.Features.Lookups.Queries.GetSkillsQuery query,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query, cancellationToken);
        return result.ToActionResult();
    }
}

