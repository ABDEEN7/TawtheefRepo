using System.Security.Claims;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Tawtheef.Application.Common.Security;
using Tawtheef.Application.Common.Security.Tawtheef.Application.Common.Security;
using Tawtheef.Application.Features.Authenticator.Queries;
using Tawtheef.Domain.Constants;
using Tawtheef.Infrastructure.Extensions;

namespace Operations.API.Controllers;

[ApiController]
[Route("api/[controller]/profile")]
[Microsoft.AspNetCore.Authorization.Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class UserController(IMediator mediator) : ControllerBase
{
    private Result<Guid> UserId => User.FindFirst(ClaimTypes.NameIdentifier)?.Value switch
    {
        null => Result.Fail<Guid>(ErrorsCodes.InvalidUserIdentifier),
        var id => Result.Ok(Guid.Parse(id))
    };
    
    [HttpGet]
    [AuthorizePermission(PermissionKeys.Users.View)]
    public async Task<IActionResult> GetProfile()
    {
        if(UserId.IsFailed)
            return Unauthorized(UserId.Errors);
        var result = await mediator.Send(new GetUserProfileQuery{ UserId = UserId.Value});
        return result.ToActionResult();
    }
}
