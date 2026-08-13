using Application.Operation.Features.Employee.ManagementMajorSkill.Majors.Commands;
using Application.Operation.Features.Employee.ManagementMajorSkill.Majors.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Tawtheef.Infrastructure.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Tawtheef.Application.Common.Security;

namespace Operations.API.Controllers.Employee;

[ApiController]
[Route("api/[controller]")]
[Microsoft.AspNetCore.Authorization.Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class MajorsManagementController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [Route("GetMainMajors")]
    [AuthorizePermission(PermissionKeys.MajorSkills.View, PermissionKeys.MajorSkills.Manage)]
    public async Task<IActionResult> GetMainMajors([FromQuery] GetMainMajorsQuery query,CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query,cancellationToken);
        return result.ToActionResult();
    }
    [HttpGet]
    [Route("GetSubMajors")]
    [AuthorizePermission(PermissionKeys.MajorSkills.View, PermissionKeys.MajorSkills.Manage)]
    public async Task<IActionResult> GetSubMajors([FromQuery] GetSubMajorsQuery query,CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query,cancellationToken);
        return result.ToActionResult();
    }
    [HttpPost]
    [AuthorizePermission(PermissionKeys.MajorSkills.Manage)]
    public async Task<IActionResult> CreateMajor([FromBody] CreateMajorCommand command)
    {
        var result = await mediator.Send(command);
        return result.ToActionResult();
    }

    [HttpPut]
    [AuthorizePermission(PermissionKeys.MajorSkills.Manage)]
    public async Task<IActionResult> UpdateMajor([FromBody] UpdateMajorCommand command)
    {
        var result = await mediator.Send(command);
        return result.ToActionResult();
    }

    [HttpPut]
    [Route("change-activation")]
    [AuthorizePermission(PermissionKeys.MajorSkills.Manage)]
    public async Task<IActionResult> ChangeActiveStatusMajor([FromBody] ChangeActiveStatusMajorCommand command)
    {
        var result = await mediator.Send(command);
        return result.ToActionResult();
    }
}

