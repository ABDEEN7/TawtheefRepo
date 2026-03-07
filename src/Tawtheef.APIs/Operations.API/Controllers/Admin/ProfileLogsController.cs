using Application.Operation.Features.Admin.ProfileLogs.Queries;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Tawtheef.Application.Common;
using Tawtheef.Application.Common.Security;
using Tawtheef.Infrastructure.Extensions;

namespace Operations.API.Controllers.Admin;

[ApiController]
[Route("api/profile-logs")]
[Microsoft.AspNetCore.Authorization.Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class ProfileLogsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [AuthorizePermission(PermissionKeys.ProfileLogs.View)]
    public async Task<IActionResult> Get([FromQuery] GetProfileLogsQuery query)
    {
        var result = await mediator.Send(query);
        return result.ToActionResult();
    }
}

