using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tawtheef.Application.Features.Operations.Admin.Offices.Commands;
using Tawtheef.Application.Features.Operations.Admin.Offices.Queries;
using Tawtheef.Infrastructure.Extensions;

namespace Operations.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class OfficesController(IMediator mediator) : ControllerBase
{
    [HttpGet("list-offices")]
    public async Task<IActionResult> ListOffices([FromQuery] GetListOfficesQuery query)
    {
        var result = await mediator.Send(query);
        return result.ToActionResult();
    }

    [HttpGet("office-details/{id:guid}")]
    public async Task<IActionResult> OfficeDetails(Guid id)
    {
        var result = await mediator.Send(new GetOfficeDetailsQuery(id));
        return result.ToActionResult();
    }

    [HttpGet("lookups/countries")]
    public async Task<IActionResult> ListCountries()
    {
        var result = await mediator.Send(new GetOfficeCountriesQuery());
        return result.ToActionResult();
    }

    [HttpPost("create-office")]
    public async Task<IActionResult> CreateOffice([FromBody] CreateOfficeCommand command)
    {
        var result = await mediator.Send(command);
        return result.ToActionResult();
    }

    [HttpPut("update-office/{id:guid}")]
    public async Task<IActionResult> UpdateOffice(Guid id, [FromBody] UpdateOfficeCommand command)
    {
        var result = await mediator.Send(command with { Id = id });
        return result.ToActionResult();
    }

    [HttpDelete("delete-office/{id:guid}")]
    public async Task<IActionResult> DeleteOffice(Guid id)
    {
        var result = await mediator.Send(new DeleteOfficeCommand(id));
        return result.ToActionResult();
    }

    [HttpPut("{officeId:guid}/users/{userId:guid}/block-status")]
    public async Task<IActionResult> UpdateBlockStatus(Guid officeId, Guid userId, [FromBody] UpdateOfficeUserBlockStatusCommand command)
    {
        var result = await mediator.Send(command with { OfficeId = officeId, UserId = userId });
        return result.ToActionResult();
    }

    [HttpPut("{officeId:guid}/set-admin/{userId:guid}")]
    public async Task<IActionResult> SetOfficeAdmin(Guid officeId, Guid userId)
    {
        var result = await mediator.Send(new SetOfficeAdminCommand(officeId, userId));
        return result.ToActionResult();
    }
}
