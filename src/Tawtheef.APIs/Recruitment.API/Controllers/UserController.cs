using System.Security.Claims;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Features.Authenticator.Commands;
using Tawtheef.Application.Features.Authenticator.Queries;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Users;
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
    [HttpPost("verify/phone/request")]
    public async Task<IActionResult> RequestPhoneVerification([FromBody] RequestPhoneVerificationCommand command)
    {
        if(UserId.IsFailed) return BadRequest(UserId.Errors);
        var result = await mediator.Send(command with { UserId = UserId.Value });
        return result.ToActionResult();
    }

    [HttpPost("verify/phone/confirm")]
    public async Task<IActionResult> ConfirmPhoneVerification([FromBody] ConfirmPhoneVerificationCommand command)
    {
        if(UserId.IsFailed) return BadRequest(UserId.Errors);
        var result = await mediator.Send(command with { UserId = UserId.Value });
        return result.ToActionResult();
    }

    [HttpPost("verify/email/request")]
    public async Task<IActionResult> RequestEmailVerification([FromBody] RequestEmailVerificationCommand command)
    {
        if(UserId.IsFailed) return BadRequest(UserId.Errors);
        var result = await mediator.Send(command with { UserId = UserId.Value });
        return result.ToActionResult();
    }

    [HttpPost("verify/email/confirm")]
    public async Task<IActionResult> ConfirmEmailVerification([FromBody] ConfirmEmailVerificationCommand command)
    {
        if(UserId.IsFailed) return BadRequest(UserId.Errors);
        var result = await mediator.Send(command with { UserId = UserId.Value });
        return result.ToActionResult();
    }

    #endregion
}
