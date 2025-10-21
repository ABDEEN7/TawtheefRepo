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
using Tawtheef.Application.Features.Authenticator.Commands;
using Tawtheef.Domain.Configurations;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Users;
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
    
    [HttpPost("register")]
    public Task<IActionResult> RegisterStudent([FromBody] RegisterUserCommand command)
    {
        // var result = await mediator.Send(command with { UserType = nameof(UserTypeIds.Student) });
        // return result.ToActionResult();
        throw new NotImplementedException();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        var headers = HttpContext.Request.Headers;
        
        var result = await mediator.Send(command with {
            DeviceId = headers["x-device-id"].FirstOrDefault(),
            UserAgent = headers.UserAgent.FirstOrDefault(),
            Timezone = headers["x-device-tz"].FirstOrDefault(),
            Screen = headers["x-device-screen"].FirstOrDefault(),
            Browser = headers["x-device-platform"].FirstOrDefault(),
            DisplayName = headers["x-device-model"].FirstOrDefault(),
            Os = headers["x-device-os"].FirstOrDefault(),
            IpAddress = HttpContext.GetClientIpAddress() ?? "Unknown IP Address"
        });
        return result.ToActionResult();
    }
    
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand command)
    {
        command = command with { IpAddress = HttpContext.GetClientIpAddress() ?? "Unknown IP Address" };
        var result = await mediator.Send(command);
        return result.ToActionResult();
    }
    
    [HttpPost("forget-password")]
    public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordCommand command)
    {
        var result = await mediator.Send(command);
        return result.ToActionResult();
    }
    
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command)
    {
        var result = await mediator.Send(command);
        return result.ToActionResult();
    }
    
    [HttpPost("verify-lock-password")]
    public async Task<IActionResult> VerifyLockPassword([FromBody] VerifyLockPasswordCommand command)
    {
        var result = await mediator.Send(command);
        return result.ToActionResult();
    }

    [HttpPost("revoke-token")]
    public async Task<IActionResult> RevokeToken([FromBody] RevokeTokenCommand command)
    {
        command = command with { IpAddress = HttpContext.GetClientIpAddress() ?? "Unknown IP Address" };
        await mediator.Send(command);
        return NoContent();
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
    
    [HttpPost("resend-email")]
    public async Task<IActionResult> ResendVerificationEmail([FromBody] ResendVerificationEmailCommand command)
    {
        var result = await mediator.Send(command);
        return result.ToActionResult();
    }

    [HttpPost("via-sms")]
    public async Task<IActionResult> VerifyViaSms([FromBody] VerifyEmailViaSmsCommand command)
    {
        var result = await mediator.Send(command);
        return result.ToActionResult();
    }

    [HttpGet("external-login")]
    public IActionResult ExternalLogin([FromQuery] ExternalLoginCommand command, [FromServices] SignInManager<User> signInManager)
    {
        var provider = command.Provider;
        if (string.IsNullOrEmpty(provider))
            return BadRequest(ErrorsCodes.ExternalLoginProviderRequired);
        
        var returnUrl = command.ReturnUrl ?? Url.Content("~/");
        var redirectUrl = Url.ActionLink(nameof(ExternalLoginCallback), controller: null, values: new { returnUrl }, protocol: Request.Scheme);
        var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        return new ChallengeResult(provider, properties);
    }

    [HttpGet("/ExternalLoginCallback")]
    public async Task<IActionResult> ExternalLoginCallback([FromQuery] ExternalCallbackLoginCommand command, [FromServices] IOptions<AppConfigSettings> appConfig)
    {
        var frontEndOrigin = appConfig.Value.FrontendUrl;
        var result = await mediator.Send(command);
        
        var serializedError = JsonSerializer.Serialize(result.Error);
        var encodedOrigin = JsonSerializer.Serialize(frontEndOrigin);
        if (result.IsFailure)
        {
            var htmlError = $$"""
                                  <!doctype html><meta charset="utf-8">
                                  <script>
                                    (function() {
                                      try {
                                        if (window.opener) {
                                          window.opener.postMessage({
                                            type: 'EXTERNAL_LOGIN_ERROR',
                                            message: {{serializedError}}
                                          }, '{{encodedOrigin}}');
                                        }
                                      } catch (e) { console.error(e); }
                                      window.close();
                                    })();
                                  </script>
                              """;
            return Content(htmlError, "text/html");
        }

        var serializedUser = JsonSerializer.Serialize(result.Value, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        var htmlOk = $$"""
                           <!doctype html><meta charset="utf-8">
                           <script>
                             (function() {
                               try {
                                 if (window.opener) {
                                   window.opener.postMessage({
                                     type: 'EXTERNAL_LOGIN_SUCCESS',
                                     userData: {{serializedUser}}
                                   }, '{{encodedOrigin}}');
                                 }
                               } catch (e) { console.error(e); }
                               window.close();
                             })();
                           </script>
                       """;
        return Content(htmlOk, "text/html");
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
