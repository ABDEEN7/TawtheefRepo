using System.Security.Claims;
using System.Text;
using System.Text.Json;
using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Tawtheef.Application.Common.Constants;
using Tawtheef.Application.Common.Interfaces;
using Tawtheef.Application.Common.Models;
using Tawtheef.Application.Features.Authenticator.Commands;
using Tawtheef.Domain.Configurations.Settings;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Users;
using Tawtheef.Infrastructure.Extensions;

namespace Operations.API.Controllers
{
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

        [HttpGet("external-login")]
        public IActionResult ExternalLogin([FromQuery] ExternalLoginRequest request,
            [FromServices] SignInManager<User> signInManager)
        {
            var provider = request.Provider;
            if (string.IsNullOrEmpty(provider))
                return BadRequest(ErrorsCodes.ExternalLoginProviderRequired);

            var returnUrl = request.ReturnUrl ?? Url.Content("~/");
            var redirectUrl = Url.ActionLink(nameof(AzureExternalLoginCallback), controller: null,
                values: new { returnUrl },
                protocol: Request.Scheme);
            var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        [HttpGet("azure/external-login-callback", Name = nameof(AzureExternalLoginCallback))]
        public async Task<IActionResult> AzureExternalLoginCallback(
            [FromQuery] AzureExternalCallbackLoginCommand command,
            [FromServices] IHttpContextAccessor http,
            [FromServices] SignInManager<User> signInManager,
            [FromServices] IExternalTokenReader tokenReader,
            [FromServices] IOptions<AppConfigSettings> appConfig)
        {
            var result = await mediator.Send(command);

            var spaOrigin = GetOriginOnly(appConfig.Value.FrontendUrl);
            var spaCallback = $"{spaOrigin}/auth/popup-callback";

            object message = result.IsFailure
                ? new { type = ExternalLoginMessageTypes.Error, message = result.Error }
                : new { type = ExternalLoginMessageTypes.Success, userData = result.Value };

            var json = JsonSerializer.Serialize(message,
                new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

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
}
