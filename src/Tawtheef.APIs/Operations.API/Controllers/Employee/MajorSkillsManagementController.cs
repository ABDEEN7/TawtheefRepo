using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tawtheef.Application.Common.Constants;
using Tawtheef.Application.Features.Lookups.Queries;
using Tawtheef.Application.Features.Operations.Employee.ManagementMajorSkill.Mapping.Commands;
using Tawtheef.Application.Features.Operations.Employee.ManagementMajorSkill.Mapping.Queries;
using Tawtheef.Infrastructure.Extensions;
using Tawtheef.Infrastructure.Services.Authorization;

namespace Operations.API.Controllers.Employee;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = PermissionPolicyProvider.PolicyPrefix + PermissionNames.MajorSkillsManage)]
public class MajorSkillsManagementController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllMajorSkills([FromQuery] GetMajorSkillsQuery query, CancellationToken cancellationToken)
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
    
    
    [HttpGet("lookups/majors")]
    public async Task<IActionResult> GetMajors([FromQuery] GetMajorsQuery query)
    {
        //get language from header
        var language = Request.Headers.AcceptLanguage.ToString();
        var result = await mediator.Send(query with {Language = language});
        return result.ToActionResult();
    }

    [HttpGet("lookups/sub-majors")]
    public async Task<IActionResult> GetMajors([FromQuery] GetSubMajorsQuery query)
    {
        var result = await mediator.Send(query);
        return result.ToActionResult();
    }
}
