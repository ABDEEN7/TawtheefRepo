using Application.Operation.Features.Employee.Locations.Queries;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Tawtheef.Application.Common.Security;
using Tawtheef.Infrastructure.Extensions;
using Application.Operation.Features.Employee.Locations.Commands;
using Application.Operation.Features.Employee.Locations.DTOs;

namespace Operations.API.Controllers.Employee;

[ApiController]
[Route("api/[controller]")]
[Microsoft.AspNetCore.Authorization.Authorize(
    AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public sealed class LocationsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [AuthorizePermission(PermissionKeys.Locations.View)]
    public async Task<IActionResult> ListLocations([FromQuery] ListLocationsQuery query)
    {
        var result = await mediator.Send(query);
        return result.ToActionResult();
    }

    [HttpPost]
    [AuthorizePermission(PermissionKeys.Locations.Manage)]
    public async Task<IActionResult> CreateLocation([FromBody] SaveLocationDto location)
    {
        var result = await mediator.Send(new CreateLocationCommand(location));
        return result.ToActionResult();
    }

    [HttpPut("{id:guid}")]
    [AuthorizePermission(PermissionKeys.Locations.Manage)]
    public async Task<IActionResult> UpdateLocation(Guid id, [FromBody] SaveLocationDto location)
    {
        var result = await mediator.Send(new UpdateLocationCommand(id, location));
        return result.ToActionResult();
    }

    [HttpDelete("{id:guid}")]
    [AuthorizePermission(PermissionKeys.Locations.Manage)]
    public async Task<IActionResult> DeleteLocation(Guid id)
    {
        var result = await mediator.Send(new DeleteLocationCommand(id));
        return result.ToActionResult();
    }
}
