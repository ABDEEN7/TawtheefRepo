using System.Security.Claims;
using System.Text;
using System.Text.Json;
using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using Tawtheef.Application.Common.Constants;
using Tawtheef.Application.Common.Models;
using Tawtheef.Application.Features.Authenticator.Commands;
using Tawtheef.Domain.Configurations.Settings;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Users;
using Tawtheef.Infrastructure;
using Tawtheef.Infrastructure.Extensions;

namespace Recruitment.API.Controllers;

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
        var result = await mediator.Send(command);
        return result.ToActionResult();
    }

    [HttpPost("send-otp")]
    public async Task<IActionResult> VerifyOtp([FromBody] SendOtpCommand command)
    {
        var result = await mediator.Send(command);
        return result.ToActionResult();
    }
    [HttpPost("resend-otp")]
    public async Task<IActionResult> VerifyOtp([FromBody] ResendOtpCommand command)
    {
        var result = await mediator.Send(command);
        return result.ToActionResult();
    }
    [EnableRateLimiting(LimitsPolicyKeys.VerificationPolicy)]
    [HttpPost("verify-otp")]
    public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpCommand command)
    {
        var result = await mediator.Send(command);
        return result.ToActionResult();
    }

    [HttpGet("external-login")]
    public IActionResult ExternalLogin([FromQuery] ExternalLoginRequest request,
        [FromServices] SignInManager<User> signInManager)
    {
        var provider = request.Provider;
        if (string.IsNullOrEmpty(provider))
            return BadRequest(ErrorsCodes.ExternalLoginProviderRequired);

        var returnUrl = request.ReturnUrl ?? Url.Content("~/");
        var redirectUrl = Url.ActionLink(nameof(GoogleExternalLoginCallback), controller: null, values: new { returnUrl },
            protocol: Request.Scheme);
        var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        return new ChallengeResult(provider, properties);
    }

    [HttpGet("google/external-login-callback", Name = nameof(GoogleExternalLoginCallback))]
    public async Task<IActionResult> GoogleExternalLoginCallback(
        [FromQuery] GoogleExternalCallbackLoginCommand command,
        [FromServices] IOptions<AppConfigSettings> appConfig)
    {
        var result = await mediator.Send(command);

        var spaOrigin = GetOriginOnly(appConfig.Value.FrontendUrl);
        var spaCallback = $"{spaOrigin}/auth/popup-callback";

        object message = result.IsFailure
            ? new { type = ExternalLoginMessageTypes.Error, message = result.Error }
            : new { type = ExternalLoginMessageTypes.Success, userData = result.Value };

        var json = JsonSerializer.Serialize(message, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        var b64 = Base64UrlEncode(json);
        var url = $"{spaCallback}#payload={b64}";
        return Redirect(url);
    }
         
    [HttpGet("qatar-pass/external-login-callback", Name = nameof(QatarPassExternalLoginCallBack))]
    public async Task<IActionResult> QatarPassExternalLoginCallBack(
        [FromQuery] QatarPassExternalCallbackLoginCommand command,
        [FromServices] IOptions<AppConfigSettings> appConfig)
    {
        var result = await mediator.Send(command);
             
        var spaOrigin = GetOriginOnly(appConfig.Value.FrontendUrl);
        var spaCallback = $"{spaOrigin}/auth/popup-callback";

        object message = result.IsFailure
            ? new { type = "EXTERNAL_LOGIN_ERROR", message = result.Error }
            : new { type = "EXTERNAL_LOGIN_SUCCESS", userData = result.Value };

        var json = JsonSerializer.Serialize(message, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        var b64 = Base64UrlEncode(json);
        var url = $"{spaCallback}#payload={b64}";
        return Redirect(url);
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

    #region Utils

    private static string GetOriginOnly(string url)
    {
        var uri = new Uri(url);
        return uri.GetLeftPart(UriPartial.Authority);
    }

    private static string Base64UrlEncode(string input)
    {
        var bytes = Encoding.UTF8.GetBytes(input);
        return Convert.ToBase64String(bytes)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }

    #endregion
}
