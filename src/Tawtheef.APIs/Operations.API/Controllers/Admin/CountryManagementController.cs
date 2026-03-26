using Application.Operation.Features.Admin.Countries.Commands;
using Application.Operation.Features.Admin.Countries.Queries;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Tawtheef.Application.Common.Security;
using Tawtheef.Infrastructure.Extensions;

namespace Operations.API.Controllers.Admin;

[ApiController]
[Route("api/[controller]")]
[Microsoft.AspNetCore.Authorization.Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class CountryManagementController(IMediator mediator) : ControllerBase
{
    [HttpGet("list-countries")]
    [AuthorizePermission(PermissionKeys.Countries.View)]
    public async Task<IActionResult> ListCountries([FromQuery] GetListCountriesQuery query)
    {
        var result = await mediator.Send(query);
        return result.ToActionResult();
    }

    [HttpPut("{id:guid}/status")]
    [AuthorizePermission(PermissionKeys.Countries.Manage)]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateCountryStatusCommand command)
    {
        var result = await mediator.Send(command with { CountryId = id });
        return result.ToActionResult();
    }
}

