using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Application.Recruitment.Features.Authenticator.Commands.QatarLogin;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Tawtheef.Application.Common.Constants;
using Tawtheef.Application.Common.Models;
using Tawtheef.Application.Features.Authenticator.Commands;
using Tawtheef.Domain.Configurations.Settings;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Users;
using Tawtheef.Infrastructure.Extensions;

namespace Recruitment.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IMediator mediator) : ControllerBase
{
    private Result<Guid> UserId => User.FindFirst(ClaimTypes.NameIdentifier)?.Value switch
    {
        null => Result.Fail<Guid>(ErrorsCodes.InvalidUserIdentifier),
        var id when Guid.TryParse(id, out var guid) => Result.Ok(guid),
        _ => Result.Fail<Guid>(ErrorsCodes.InvalidUserIdentifier)
    };
    
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand command)
    {
        command = command with { IpAddress = HttpContext.GetClientIpAddress() ?? "Unknown IP Address" };
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
        [FromQuery] RequestGoogleExternalCallbackLoginCommand req,
        [FromServices] IOptions<AppConfigSettings> appConfig)
    {
        var result = await mediator.Send(new GoogleExternalCallbackLoginCommand(UserTypeIds.Applicant, req.ReturnUrl, req.RemoteError));

        var spaOrigin = GetOriginOnly(appConfig.Value.FrontendUrl);
        var spaCallback = $"{spaOrigin}/auth/popup-callback";

        object message = result.IsFailed
            ? new { type = ExternalLoginMessageTypes.Error, message = result.Errors[0].Message }
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

        object message = result.IsFailed
            ? new { type = ExternalLoginMessageTypes.Error, message = result.Errors[0].Message }
            : new { type = ExternalLoginMessageTypes.Success, userData = result.Value };

        var json = JsonSerializer.Serialize(message, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        var b64 = Base64UrlEncode(json);
        var url = $"{spaCallback}#payload={b64}";
        return Redirect(url);
    }

    [HttpPost("qatar-resident/request-otp")]
    public async Task<IActionResult> RequestQatarResidentOtp([FromBody] RequestQatarResidentOtpCommand command)
    {
        var result = await mediator.Send(command);
        return result.ToActionResult();
    }

    [HttpPost("qatar-resident/verify-otp")]
    public async Task<IActionResult> VerifyQatarResidentOtp([FromBody] VerifyQatarResidentOtpCommand command)
    {
        var result = await mediator.Send(command);
        return result.ToActionResult();
    }


    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        if (UserId.IsFailed)
            return BadRequest(UserId.Errors);

        var result = await mediator.Send(new LogoutCommand(UserId.Value));
        if (result.IsFailed)
            return BadRequest(result.Errors);
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

