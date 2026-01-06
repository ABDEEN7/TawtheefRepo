using System.Security.Claims;
using Cortex.Mediator;
using FluentResults;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Tawtheef.Application.Common;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Features.Authenticator.Commands;
using Tawtheef.Application.Features.Authenticator.Queries;
using Tawtheef.Application.Features.Recruitment.Profile.Queries;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Users;
using Tawtheef.Infrastructure;
using Tawtheef.Infrastructure.Extensions;

namespace Recruitment.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class UserController(IMediator mediator) : ControllerBase
{
    private Result<Guid> UserId => User.FindFirst(ClaimTypes.NameIdentifier)?.Value switch
    {
        null => Result.Fail<Guid>(ErrorsCodes.InvalidUserIdentifier),
        var id => Result.Ok(Guid.Parse(id))
    };
    
    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        if(UserId.IsFailed)
            return Unauthorized(UserId.Errors);
        var result = await mediator.Send(new GetUserProfileQuery{ UserId = UserId.Value});
        return result.ToActionResult();
    }
    [HttpGet("profile/detail")]
    public async Task<IActionResult> GetProfileDetail(CancellationToken ct)
    {
        if (UserId.IsFailed) return Unauthorized(UserId.Errors);
        var result = await mediator.Send(new GetMyProfileDetailQuery { UserId = UserId.Value }, ct);
        return result.ToActionResult();
    }

    [HttpGet("profile/review-summary")]
    public async Task<IActionResult> GetMyProfileReviewSummary(CancellationToken ct)
    {
        if (UserId.IsFailed)
            return Unauthorized(UserId.Errors);

        var result = await mediator.Send(new GetMyProfileReviewSummaryQuery(UserId.Value), ct);
        return result.ToActionResult();
    }
        
    [HttpGet("/api/me/bootstrap")]
    public async Task<IActionResult> Bootstrap(
        [FromServices] UserManager<User> userManager,
        [FromServices] IProfileCompletenessService pcs,
        CancellationToken ct)
    {
        var user = await userManager.GetUserAsync(User);
        if (user is null) return Unauthorized();

        var profile = await pcs.EvaluateAsync(user.Id, ct);
        return Ok(profile);
    }
    

    #region Verification Actions
    [HttpPost("update/phone")]
    [EnableRateLimiting(LimitsPolicyKeys.VerificationRequestPolicy)]
    public async Task<IActionResult> RequestUpdatePhone([FromBody] RequestUpdatePhoneCommand command)
    {
        if(UserId.IsFailed) return BadRequest(UserId.Errors);
        var result = await mediator.Send(command with { UserId = UserId.Value });
        return result.ToActionResult();
    }
    
    [HttpPost("verify/phone/request")]
    [EnableRateLimiting(LimitsPolicyKeys.VerificationRequestPolicy)]
    public async Task<IActionResult> RequestPhoneVerification([FromBody] RequestPhoneVerificationCommand command)
    {
        if(UserId.IsFailed) return BadRequest(UserId.Errors);
        var result = await mediator.Send(command with { UserId = UserId.Value });
        return result.ToActionResult();
    }

    [HttpPost("verify/phone/confirm")]
    [EnableRateLimiting(LimitsPolicyKeys.VerificationConfirmationPolicy)]
    public async Task<IActionResult> ConfirmPhoneVerification([FromBody] ConfirmPhoneVerificationCommand command)
    {
        if(UserId.IsFailed) return BadRequest(UserId.Errors);
        var result = await mediator.Send(command with { UserId = UserId.Value });
        return result.ToActionResult();
    }

    [HttpPost("verify/email/request")]
    [EnableRateLimiting(LimitsPolicyKeys.VerificationRequestPolicy)]
    public async Task<IActionResult> RequestEmailVerification([FromBody] RequestEmailVerificationCommand command)
    {
        if(UserId.IsFailed) return BadRequest(UserId.Errors);
        var result = await mediator.Send(command with { UserId = UserId.Value });
        return result.ToActionResult();
    }

    [HttpPost("verify/email/confirm")]
    [EnableRateLimiting(LimitsPolicyKeys.VerificationConfirmationPolicy)]
    public async Task<IActionResult> ConfirmEmailVerification([FromBody] ConfirmEmailVerificationCommand command)
    {
        if(UserId.IsFailed) return BadRequest(UserId.Errors);
        var result = await mediator.Send(command with { UserId = UserId.Value });
        return result.ToActionResult();
    }

    #endregion
}
