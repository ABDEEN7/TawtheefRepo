using System.Security.Claims;
using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Tawtheef.Application.Common.Constants;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Features.Authenticator.DTOs.Responses;
using Tawtheef.Application.Features.Authenticator.Queries;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Users;
using Tawtheef.Infrastructure.Extensions;
using Tawtheef.Infrastructure.Services.Authorization;

namespace Recruitment.API.Controllers;

[ApiController]
[Route("api/[controller]/profile")]
[Authorize(Policy = PermissionPolicyProvider.POLICY_PREFIX + PermissionNames.ProfileManage)]
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
        
    [HttpGet("/api/me/bootstrap")]
    public async Task<ActionResult<AuthResponse>> Bootstrap(
        [FromServices] UserManager<User> userManager,
        [FromServices] IProfileCompletenessService pcs,
        CancellationToken ct)
    {
        var user = await userManager.GetUserAsync(User);
        if (user is null) return Unauthorized();

        (bool isComplete, string[] missing) = await pcs.EvaluateAsync(user.Id, ct);
        var prefill = await pcs.BuildPrefillAsync(user, ct);

        return Ok(new AuthResponse(!isComplete, null,null,missing,prefill));
    }
}
