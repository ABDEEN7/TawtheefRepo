using Application.Operation.Features.Admin.Cities.Commands;
using Application.Operation.Features.Admin.Cities.Queries;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Tawtheef.Application.Common.Security;
using Tawtheef.Infrastructure.Extensions;

namespace Operations.API.Controllers.Admin;

[ApiController]
[Route("api/[controller]")]
[Microsoft.AspNetCore.Authorization.Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class CityManagementController(IMediator mediator) : ControllerBase
{
    [HttpGet("list-cities")]
    [AuthorizePermission(PermissionKeys.Cities.View)]
    public async Task<IActionResult> ListCities([FromQuery] GetListCitiesQuery query)
    {
        var result = await mediator.Send(query);
        return result.ToActionResult();
    }

    [HttpPost("create-city")]
    [AuthorizePermission(PermissionKeys.Cities.Manage)]
    public async Task<IActionResult> CreateCity([FromBody] CreateCityCommand command)
    {
        var result = await mediator.Send(command);
        return result.ToActionResult();
    }

    [HttpPut("update-city/{id:guid}")]
    [AuthorizePermission(PermissionKeys.Cities.Manage)]
    public async Task<IActionResult> UpdateCity(Guid id, [FromBody] UpdateCityCommand command)
    {
        var result = await mediator.Send(command with { Id = id });
        return result.ToActionResult();
    }

    [HttpPut("{id:guid}/status")]
    [AuthorizePermission(PermissionKeys.Cities.Manage)]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateCityStatusCommand command)
    {
        var result = await mediator.Send(command with { CityId = id });
        return result.ToActionResult();
    }
}
