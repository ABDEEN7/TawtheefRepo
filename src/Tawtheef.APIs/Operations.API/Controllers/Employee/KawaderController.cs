using Application.Operation.Features.Employee.Kawader.Commands;
using Application.Operation.Features.Employee.Kawader.Queries;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Tawtheef.Application.Common.Security;
using Tawtheef.Infrastructure.Extensions;

namespace Operations.API.Controllers.Employee;

[ApiController]
[Route("api/[controller]")]
[Microsoft.AspNetCore.Authorization.Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class KawaderController(IMediator mediator) : ControllerBase
{
    [HttpPost("upload")]
    [AuthorizePermission(PermissionKeys.Kawader.Manage)]
    public async Task<IActionResult> Upload([FromForm] UploadKawaderUserCommand command)
    {
        var result = await mediator.Send(command);
        return result.ToActionResult();
    }

    [HttpGet]
    [AuthorizePermission(PermissionKeys.Kawader.Manage)]
    public async Task<IActionResult> Get([FromQuery] GetKawaderUserQuery query)
    {
        var result = await mediator.Send(query);
        return Ok(result);
    }
}

