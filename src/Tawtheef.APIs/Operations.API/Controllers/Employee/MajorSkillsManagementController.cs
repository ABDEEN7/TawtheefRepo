using Application.Operation.Features.Employee.ManagementMajorSkill.Mapping.Commands;
using Application.Operation.Features.Employee.ManagementMajorSkill.Mapping.Queries;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Tawtheef.Application.Common;
using Tawtheef.Application.Common.Security;
using Tawtheef.Application.Features.Lookups.Queries;
using Tawtheef.Infrastructure.Extensions;

namespace Operations.API.Controllers.Employee;

[ApiController]
[Route("api/[controller]")]
[Microsoft.AspNetCore.Authorization.Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class MajorSkillsManagementController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [AuthorizePermission(PermissionKeys.MajorSkills.Manage)]
    public async Task<IActionResult> GetAllMajorSkills([FromQuery] GetMajorSkillsQuery query, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query, cancellationToken);
        return result.ToActionResult();
    }

    [HttpGet("{id:guid}")]
    [AuthorizePermission(PermissionKeys.MajorSkills.Manage)]
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
    [AuthorizePermission(PermissionKeys.MajorSkills.Manage)]
    public async Task<IActionResult> UpdateMajorSkill([FromBody] UpdateMajorSkillCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPut]
    [Route("change-activation")]
    [AuthorizePermission(PermissionKeys.MajorSkills.Manage)]
    public async Task<IActionResult> ChangeMajorSkillActivation([FromBody] ChangeActiveStatusMajorSkillCommand command,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return result.ToActionResult();
    }
    
    
    [HttpGet("lookups/majors")]
    [AuthorizePermission(PermissionKeys.MajorSkills.Manage)]
    public async Task<IActionResult> GetMajors([FromQuery] GetMainMajorsQuery query)
    {
        //get language from header
        var language = Request.Headers.AcceptLanguage.ToString();
        var result = await mediator.Send(query with {Language = language});
        return result.ToActionResult();
    }

    [HttpGet("lookups/sub-majors")]
    [AuthorizePermission(PermissionKeys.MajorSkills.Manage)]
    public async Task<IActionResult> GetMajors([FromQuery] GetSubMajorsQuery query)
    {
        var result = await mediator.Send(query);
        return result.ToActionResult();
    }
}

