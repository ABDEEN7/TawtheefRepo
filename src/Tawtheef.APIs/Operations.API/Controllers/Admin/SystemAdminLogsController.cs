using Application.Operation.Features.Admin.SystemAdminLogs.Queries;
using Application.Operation.Features.Admin.Users.Queries;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Tawtheef.Application.Common.Security;
using Tawtheef.Infrastructure.Extensions;

namespace Operations.API.Controllers.Admin;

[ApiController]
[Route("api/system-admin-logs")]
[Microsoft.AspNetCore.Authorization.Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class SystemAdminLogsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [AuthorizePermission(PermissionKeys.ProfileLogs.View)]
    public async Task<IActionResult> Get([FromQuery] GetSystemAdminLogsQuery query)
    {
        var result = await mediator.Send(query);
        return result.ToActionResult();
    } 

    [HttpGet("lookups")]
    [AuthorizePermission(PermissionKeys.ProfileLogs.View)]
    public async Task<IActionResult> GetUsersLookup()
    {
        var result = await mediator.Send(new GetUsersLookupQuery());
        return result.ToActionResult();
    }
}

