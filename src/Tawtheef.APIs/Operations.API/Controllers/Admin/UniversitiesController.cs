using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tawtheef.Application.Common.Constants;
using Tawtheef.Application.Features.Lookups.Queries;
using Tawtheef.Application.Features.Operations.Admin.Universities.Commands;
using Tawtheef.Application.Features.Operations.Admin.Universities.Queries;
using Tawtheef.Infrastructure.Extensions;
using Tawtheef.Infrastructure.Services.Authorization;

namespace Operations.API.Controllers.Admin;

[ApiController]
[Route("api/[controller]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class UniversitiesController(IMediator mediator) : ControllerBase
{
    [HttpGet("list-universities")]
    [Authorize(Policy = PermissionPolicyProvider.PolicyPrefix + PermissionNames.UniversitiesView)]
    public async Task<IActionResult> ListUniversities([FromQuery] GetListUniversitiesQuery query)
    {
        var result = await mediator.Send(query);
        return result.ToActionResult();
    }

    [HttpGet("university-details/{id:guid}")]
    [Authorize(Policy = PermissionPolicyProvider.PolicyPrefix + PermissionNames.UniversitiesView)]
    public async Task<IActionResult> UniversityDetails(Guid id)
    {
        var result = await mediator.Send(new GetUniversityDetailsQuery(id));
        return result.ToActionResult();
    }

    [HttpPost("create-university")]
    [Authorize(Policy = PermissionPolicyProvider.PolicyPrefix + PermissionNames.UniversitiesManage)]
    public async Task<IActionResult> CreateUniversity([FromForm] CreateUniversityCommand command)
    {
        var result = await mediator.Send(command);
        return result.ToActionResult();
    }

    [HttpPut("update-university/{id:guid}")]
    [Authorize(Policy = PermissionPolicyProvider.PolicyPrefix + PermissionNames.UniversitiesManage)]
    public async Task<IActionResult> UpdateUniversity(Guid id, [FromForm] UpdateUniversityCommand command)
    {
        var result = await mediator.Send(command with { Id = id });
        return result.ToActionResult();
    }

    [HttpPut("{id:guid}/status")]
    [Authorize(Policy = PermissionPolicyProvider.PolicyPrefix + PermissionNames.UniversitiesManage)]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateUniversityStatusCommand command)
    {
        var result = await mediator.Send(command with { UniversityId = id });
        return result.ToActionResult();
    }

    [HttpGet("lookups/countries")]
    [Authorize(Policy = PermissionPolicyProvider.PolicyPrefix + PermissionNames.UniversitiesView)]
    public async Task<IActionResult> ListCountries()
    {
        var language = Request.Headers.AcceptLanguage.ToString();
        var result = await mediator.Send(new GetCountriesQuery() { Language = language });
        return result.ToActionResult();
    }

    [HttpGet("lookups/cities")]
    [Authorize(Policy = PermissionPolicyProvider.PolicyPrefix + PermissionNames.UniversitiesView)]
    public async Task<IActionResult> ListCities([FromQuery] Guid countryId)
    {
        var language = Request.Headers.AcceptLanguage.ToString();
        var result = await mediator.Send(new GetCitiesByCountryQuery(countryId) { Language = language });
        return result.ToActionResult();
    }
}
