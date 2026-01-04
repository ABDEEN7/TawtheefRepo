using System.Security.Claims;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Tawtheef.Application.Common.Security;
using Tawtheef.Application.Common.Security.Tawtheef.Application.Common.Security;
using Tawtheef.Application.Features.Lookups.Queries;
using Tawtheef.Application.Features.Operations.Admin.Offices.Commands;
using Tawtheef.Application.Features.Operations.Admin.Offices.Queries;
using Tawtheef.Domain.Constants;
using Tawtheef.Infrastructure.Extensions;

namespace Operations.API.Controllers.Admin;

[ApiController]
[Route("api/[controller]")]
[Microsoft.AspNetCore.Authorization.Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class OfficesController(IMediator mediator) : ControllerBase
{
    private Result<Guid> UserId => User.FindFirst(ClaimTypes.NameIdentifier)?.Value switch
    {
        null => Result.Fail<Guid>(ErrorsCodes.InvalidUserIdentifier),
        var id => Result.Ok(Guid.Parse(id))
    };
    
    [HttpGet("list-offices")]
    [AuthorizePermission(PermissionKeys.Offices.View)]
    public async Task<IActionResult> ListOffices([FromQuery] GetListOfficesQuery query)
    {
        var result = await mediator.Send(query);
        return result.ToActionResult();
    }

    [HttpGet("office-details/{id:guid}")]
    [AuthorizePermission(PermissionKeys.Offices.View)]
    public async Task<IActionResult> OfficeDetails(Guid id)
    {
        var result = await mediator.Send(new GetOfficeDetailsQuery(id));
        return result.ToActionResult();
    }

    [HttpGet("lookups/countries")]
    [AuthorizePermission(PermissionKeys.Offices.View)]
    public async Task<IActionResult> ListCountries()
    {
        var language = Request.Headers.AcceptLanguage.ToString();
        var result = await mediator.Send(new GetCountriesQuery() with { Language = language });
        return result.ToActionResult();
    }

    [HttpPost("create-office")]
    [AuthorizePermission(PermissionKeys.Offices.Manage)]
    public async Task<IActionResult> CreateOffice([FromBody] CreateOfficeCommand command)
    {
        var result = await mediator.Send(command);
        return result.ToActionResult();
    }

    [HttpPut("update-office/{id:guid}")]
    [AuthorizePermission(PermissionKeys.Offices.Manage)]
    public async Task<IActionResult> UpdateOffice(Guid id, [FromBody] UpdateOfficeCommand command)
    {
        var result = await mediator.Send(command with { Id = id });
        return result.ToActionResult();
    }

    [HttpDelete("delete-office/{id:guid}")]
    [AuthorizePermission(PermissionKeys.Offices.Manage)]
    public async Task<IActionResult> DeleteOffice(Guid id)
    {
        if(UserId.IsFailed) return BadRequest(UserId.Errors);
        
        var result = await mediator.Send(new DeleteOfficeCommand(UserId.Value, id));
        return result.ToActionResult();
    }

    [HttpPut("{officeId:guid}/users/{userId:guid}/block-status")]
    [AuthorizePermission(PermissionKeys.Offices.Manage)]
    public async Task<IActionResult> UpdateBlockStatus(Guid officeId, Guid userId, [FromBody] BlockOfficeUserCommand command)
    {
        var result = await mediator.Send(command with { OfficeId = officeId, UserId = userId });
        return result.ToActionResult();
    }

    [HttpPut("{officeId:guid}/set-admin/{userId:guid}")]
    [AuthorizePermission(PermissionKeys.Offices.Manage)]
    public async Task<IActionResult> SetOfficeAdmin(Guid officeId, Guid userId)
    {
        var result = await mediator.Send(new ChangeOfficeAdminCommand(officeId, userId));
        return result.ToActionResult();
    }
}
