using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tawtheef.Application.Common.Constants;
using Tawtheef.Application.Features.Operations.Admin.Countries.Commands;
using Tawtheef.Application.Features.Operations.Admin.Countries.Queries;
using Tawtheef.Infrastructure.Extensions;
using Tawtheef.Infrastructure.Services.Authorization;

namespace Operations.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class CountryManagementController(IMediator mediator) : ControllerBase
{
    [HttpGet("list-countries")]
    [Authorize(Policy = PermissionPolicyProvider.PolicyPrefix + PermissionNames.CountriesView)]
    public async Task<IActionResult> ListCountries([FromQuery] GetListCountriesQuery query)
    {
        var result = await mediator.Send(query);
        return result.ToActionResult();
    }

    [HttpPut("{id:guid}/status")]
    [Authorize(Policy = PermissionPolicyProvider.PolicyPrefix + PermissionNames.CountriesManage)]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateCountryStatusCommand command)
    {
        var result = await mediator.Send(command with { CountryId = id });
        return result.ToActionResult();
    }
}
