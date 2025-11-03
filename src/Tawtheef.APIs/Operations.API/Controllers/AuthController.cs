using System.Security.Claims;
using System.Text.Json;
using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using Tawtheef.Application.Common.Constants;
using Tawtheef.Application.Common.Interfaces;
using Tawtheef.Application.Common.Models;
using Tawtheef.Application.Features.Authenticator.Commands;
using Tawtheef.Application.Features.Authenticator.DTOs;
using Tawtheef.Domain.Configurations;
using Tawtheef.Domain.Configurations.Settings;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Users;
using Tawtheef.Infrastructure;
using Tawtheef.Infrastructure.Extensions;
using Tawtheef.Infrastructure.Utils;

namespace Recruitment.API.Controllers
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
             var redirectUrl = Url.ActionLink(nameof(AzureExternalLoginCallback), controller: null, values: new { returnUrl },
                 protocol: Request.Scheme);
             var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
             return new ChallengeResult(provider, properties);
         }

         [HttpGet("azure/external-login-callback", Name = nameof(AzureExternalLoginCallback))]
         public async Task<IActionResult> AzureExternalLoginCallback(
             [FromRoute] AzureExternalCallbackLoginCommand command,
             [FromServices] IHttpContextAccessor http,
             [FromServices] IExternalTokenReader tokenReader,
             [FromServices] IOptions<AppConfigSettings> appConfig)
         {
             var (idToken, accessToken) = await tokenReader.ReadAsync(http.HttpContext!, AuthSchemes.AppCookie);
             if (string.IsNullOrWhiteSpace(idToken))
                 return BadRequest("No id token found");
             
             var result = await mediator.Send(command with {IdToken = idToken, AccessToken = accessToken});
             var frontEndOrigin = appConfig.Value.FrontendUrl;
             if (result.IsFailure)
             {
                 var message = new
                 {
                     type = "EXTERNAL_LOGIN_ERROR",
                     message = result.Error
                 };
                 return HtmlPopupCloseScript.Create(message, frontEndOrigin);
             }
             var messageOk  = new
             {
                 type = "EXTERNAL_LOGIN_SUCCESS",
                 userData = result.Value
             };
             return HtmlPopupCloseScript.Create(messageOk , frontEndOrigin);
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
}
