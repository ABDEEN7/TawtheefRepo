using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Tawtheef.Application.Common.Models;
using Tawtheef.Application.Features.Authenticator.Commands;
using Tawtheef.Application.Features.Authenticator.DTOs;
using Tawtheef.Domain.Configurations;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Users;
using Tawtheef.Infrastructure;
using Tawtheef.Infrastructure.Extensions;

namespace Operations.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IMediator mediator) : ControllerBase
{
    private Result<Guid> UserId => User.FindFirst(ClaimTypes.NameIdentifier)?.Value switch
    {
        null => Result.Failure<Guid>(ErrorsCodes.InvalidUserIdentifier),
        var id when Guid.TryParse(id, out var guid) => Result.Success(guid),
        _ => Result.Failure<Guid>(ErrorsCodes.InvalidUserIdentifier)
    };
    
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand command)
    {
        command = command with { IpAddress = HttpContext.GetClientIpAddress() ?? "Unknown IP Address" };
        var result = await mediator.Send(command);
        return result.ToActionResult();
    }
    
    [HttpPost("verify-otp")]
    public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpCommand command)
    {
        var result = await mediator.Send(command);
        return result.ToActionResult();
    }
    
    [EnableRateLimiting(LimitsPolicyKeys.VerificationPolicy)]
    [HttpGet("verify-account")]
    public async Task<IActionResult> VerifyAccount([FromQuery] VerifyAccountCommand command)
    {
        var result = await mediator.Send(command);
        return result.ToActionResult();
    }
    [HttpPost("azure/external-login-callback", Name = "ExternalLoginWithToken")]
    public async Task<IActionResult> ExternalLoginWithToken([FromBody] AzureExternalCallbackLoginCommand body)
    {
        var cmd = body with
        {
            ClientIp = HttpContext.GetClientIpAddress() ?? "Unknown IP Address",
            UserAgent = Request.Headers.UserAgent.ToString()
        };

        var result = await mediator.Send(cmd);
        return result.ToActionResult(); // your existing extension that maps Result<T> to IActionResult
    }
    
    
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        if (UserId.IsFailure)
            return BadRequest(UserId.Error);
        
        var result = await mediator.Send(new LogoutCommand(UserId.Value));
        if (result.IsFailure)
            return BadRequest(result.Error);
        return Ok(new { Message = "Logged out" });
    }
}
