using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tawtheef.Application.Common.Constants;
using Tawtheef.Application.Features.Operations.Employee.Kawader.Commands;
using Tawtheef.Infrastructure.Extensions;
using Tawtheef.Infrastructure.Services.Authorization;

namespace Operations.API.Controllers.Employee;

[ApiController]
[Route("api/[controller]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class KawaderController(IMediator mediator) : ControllerBase
{
    [HttpPost("upload")]
    [Authorize(Policy = PermissionPolicyProvider.PolicyPrefix + PermissionNames.KawaderManage)]
    public async Task<IActionResult> Upload([FromForm] UploadKawaderQidsCommand command)
    {
        var result = await mediator.Send(command);
        return result.ToActionResult();
    }
}
