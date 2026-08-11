using Application.Operation.Features.Employee.ManagementMajorSkill.Skills.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Tawtheef.Application.Features.Lookups.Queries;
using Tawtheef.Infrastructure.Extensions;
using GetSkillsQuery = Application.Operation.Features.Employee.ManagementMajorSkill.Skills.Queries.GetSkillsQuery;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Tawtheef.Application.Common.Security;

namespace Operations.API.Controllers.Employee;

[ApiController]
[Route("api/[controller]")]
[Microsoft.AspNetCore.Authorization.Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class SkillsManagementController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [AuthorizePermission(PermissionKeys.MajorSkills.View, PermissionKeys.MajorSkills.Manage)]
    public async Task<IActionResult> GetAllSkills([FromQuery] GetSkillsQuery query,CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query,cancellationToken);
        return result.ToActionResult();
    }
    
    [HttpPost]
    [AuthorizePermission(PermissionKeys.MajorSkills.Manage)]
    public async Task<IActionResult> CreateSkill([FromBody] CreateSkillCommand command,CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command,cancellationToken);
        return result.ToActionResult();
    }
    
    [HttpPut]
    [AuthorizePermission(PermissionKeys.MajorSkills.Manage)]
    public async Task<IActionResult> UpdateSkill([FromBody] UpdateSkillCommand command,CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command,cancellationToken);
        return result.ToActionResult();
    }
    
    [HttpPut]
    [Route("change-activation")]
    [AuthorizePermission(PermissionKeys.MajorSkills.Manage)]
    public async Task<IActionResult> ChangeActiveStatusSkill([FromBody] ChangeActiveStatusSkillCommand command,CancellationToken cancellationToken)
    {  
        var result = await mediator.Send(command,cancellationToken);
        return result.ToActionResult();
    }
    
    [HttpGet]
    [Route("lookups/skill-types")]
    [AuthorizePermission(PermissionKeys.MajorSkills.View, PermissionKeys.MajorSkills.Manage)]
    public async Task<IActionResult> GetSkillTypes([FromQuery] GetSkillTypesQuery query, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query, cancellationToken);
        return result.ToActionResult();
    }
    
    [HttpGet]
    [Route("lookups/skills")]
    [AuthorizePermission(PermissionKeys.MajorSkills.View, PermissionKeys.MajorSkills.Manage)]
    public async Task<IActionResult> GetSkills([FromQuery] Tawtheef.Application.Features.Lookups.Queries.GetSkillsQuery query,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query, cancellationToken);
        return result.ToActionResult();
    }
}

