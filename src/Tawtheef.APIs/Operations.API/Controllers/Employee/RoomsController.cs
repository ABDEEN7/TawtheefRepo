using Application.Operation.Features.Employee.Rooms.Commands;
using Application.Operation.Features.Employee.Rooms.DTOs;
using Application.Operation.Features.Employee.Rooms.Queries;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Tawtheef.Application.Common.Security;
using Tawtheef.Infrastructure.Extensions;

namespace Operations.API.Controllers.Employee;

[ApiController]
[Route("api/[controller]")]
[Microsoft.AspNetCore.Authorization.Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public sealed class RoomsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [AuthorizePermission(PermissionKeys.Rooms.View)]
    public async Task<IActionResult> ListRooms([FromQuery] ListRoomsQuery query)
    {
        var result = await mediator.Send(query);
        return result.ToActionResult();
    }

    [HttpGet("lookups/room-types")]
    [AuthorizePermission(PermissionKeys.Rooms.View, PermissionKeys.Rooms.Manage)]
    public async Task<IActionResult> GetRoomTypes()
    {
        var result = await mediator.Send(new GetRoomTypesQuery
        {
            Language = Request.Headers.AcceptLanguage.ToString()
        });
        return result.ToActionResult();
    }

    [HttpGet("lookups/room-statuses")]
    [AuthorizePermission(PermissionKeys.Rooms.View, PermissionKeys.Rooms.Manage)]
    public async Task<IActionResult> GetRoomStatuses()
    {
        var result = await mediator.Send(new GetRoomStatusesQuery
        {
            Language = Request.Headers.AcceptLanguage.ToString()
        });
        return result.ToActionResult();
    }

    [HttpGet("lookups/locations")]
    [AuthorizePermission(PermissionKeys.Rooms.View, PermissionKeys.Rooms.Manage)]
    public async Task<IActionResult> GetLocations()
    {
        var result = await mediator.Send(new GetRoomLocationsQuery());
        return result.ToActionResult();
    }

    [HttpPost]
    [AuthorizePermission(PermissionKeys.Rooms.Manage)]
    public async Task<IActionResult> CreateRoom([FromBody] CreateRoomDto room)
    {
        var result = await mediator.Send(new CreateRoomCommand(room));
        return result.ToActionResult();
    }

    [HttpPut("{id:guid}")]
    [AuthorizePermission(PermissionKeys.Rooms.Manage)]
    public async Task<IActionResult> UpdateRoom(Guid id, [FromBody] CreateRoomDto room)
    {
        var result = await mediator.Send(new UpdateRoomCommand(id, room));
        return result.ToActionResult();
    }
}
