using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tawtheef.Application.Common.Constants;
using Tawtheef.Application.Features.Operations.Admin.Languages.Commands;
using Tawtheef.Application.Features.Operations.Admin.Languages.Queries;
using Tawtheef.Infrastructure.Extensions;
using Tawtheef.Infrastructure.Services.Authorization;

namespace Operations.API.Controllers.Admin;

[ApiController]
[Route("api/[controller]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class LanguagesController(IMediator mediator) : ControllerBase
{
    [HttpGet("list-languages")]
    [Authorize(Policy = PermissionPolicyProvider.PolicyPrefix + PermissionNames.LanguagesView)]
    public async Task<IActionResult> ListLanguages([FromQuery] GetListLanguagesQuery query)
    {
        var result = await mediator.Send(query);
        return result.ToActionResult();
    }

    [HttpGet("language-details/{id:guid}")]
    [Authorize(Policy = PermissionPolicyProvider.PolicyPrefix + PermissionNames.LanguagesView)]
    public async Task<IActionResult> LanguageDetails(Guid id)
    {
        var result = await mediator.Send(new GetLanguageDetailsQuery(id));
        return result.ToActionResult();
    }

    [HttpPost("create-language")]
    [Authorize(Policy = PermissionPolicyProvider.PolicyPrefix + PermissionNames.LanguagesManage)]
    public async Task<IActionResult> CreateLanguage([FromBody] SaveLanguageCommand command)
    {
        var result = await mediator.Send(command with { Id = null });
        return result.ToActionResult();
    }

    [HttpPut("update-language/{id:guid}")]
    [Authorize(Policy = PermissionPolicyProvider.PolicyPrefix + PermissionNames.LanguagesManage)]
    public async Task<IActionResult> UpdateLanguage(Guid id, [FromBody] SaveLanguageCommand command)
    {
        var result = await mediator.Send(command with { Id = id });
        return result.ToActionResult();
    }

    [HttpPut("{id:guid}/status")]
    [Authorize(Policy = PermissionPolicyProvider.PolicyPrefix + PermissionNames.LanguagesManage)]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateLanguageStatusCommand command)
    {
        var result = await mediator.Send(command with { LanguageId = id });
        return result.ToActionResult();
    }
}
