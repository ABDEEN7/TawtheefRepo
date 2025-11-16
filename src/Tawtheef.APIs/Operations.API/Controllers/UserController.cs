using System.Security.Claims;
using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tawtheef.Application.Features.Authenticator.Queries;
using Tawtheef.Domain.Constants;
using Tawtheef.Infrastructure.Extensions;

namespace Operations.API.Controllers;

[ApiController]
[Route("api/[controller]/profile")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class UserController(IMediator mediator) : ControllerBase
{
    private Result<Guid> UserId => User.FindFirst(ClaimTypes.NameIdentifier)?.Value switch
    {
        null => Result.Failure<Guid>(ErrorsCodes.InvalidUserIdentifier),
        var id => Result.Success(Guid.Parse(id))
    };
    
    [HttpGet]
    public async Task<IActionResult> GetProfile()
    {
        if(UserId.IsFailure)
            return Unauthorized(UserId.Error);
        var result = await mediator.Send(new GetUserProfileQuery{ UserId = UserId.Value});
        return result.ToActionResult();
    }
}
