using Application.Operation.Features.Admin.Languages.Commands;
using Application.Operation.Features.Admin.Languages.Queries;
using Cortex.Mediator;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Tawtheef.Application.Common;
using Tawtheef.Application.Common.Security;
using Tawtheef.Infrastructure.Extensions;

namespace Operations.API.Controllers.Admin;

[ApiController]
[Route("api/[controller]")]
[Microsoft.AspNetCore.Authorization.Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class LanguagesController(IMediator mediator) : ControllerBase
{
    [HttpGet("list-languages")]
    [AuthorizePermission(PermissionKeys.Languages.View)]
    public async Task<IActionResult> ListLanguages([FromQuery] GetListLanguagesQuery query)
    {
        var result = await mediator.Send(query);
        return result.ToActionResult();
    }

    [HttpGet("language-details/{id:guid}")]
    [AuthorizePermission(PermissionKeys.Languages.View)]
    public async Task<IActionResult> LanguageDetails(Guid id)
    {
        var result = await mediator.Send(new GetLanguageDetailsQuery(id));
        return result.ToActionResult();
    }

    [HttpPost("create-language")]
    [AuthorizePermission(PermissionKeys.Languages.Manage)]
    public async Task<IActionResult> CreateLanguage([FromBody] SaveLanguageCommand command)
    {
        var result = await mediator.Send(command with { Id = null });
        return result.ToActionResult();
    }

    [HttpPut("update-language/{id:guid}")]
    [AuthorizePermission(PermissionKeys.Languages.Manage)]
    public async Task<IActionResult> UpdateLanguage(Guid id, [FromBody] SaveLanguageCommand command)
    {
        var result = await mediator.Send(command with { Id = id });
        return result.ToActionResult();
    }

    [HttpPut("{id:guid}/status")]
    [AuthorizePermission(PermissionKeys.Languages.Manage)]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateLanguageStatusCommand command)
    {
        var result = await mediator.Send(command with { LanguageId = id });
        return result.ToActionResult();
    }
}
