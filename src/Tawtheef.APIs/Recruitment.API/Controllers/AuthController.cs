using System.Security.Claims;
using System.Text.Json;
using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using Tawtheef.Application.Common.Models;
using Tawtheef.Application.Features.Authenticator.Commands;
using Tawtheef.Domain.Configurations;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Users;
using Tawtheef.Infrastructure;
using Tawtheef.Infrastructure.Extensions;

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

             var frontEndOrigin = appConfig.Value.FrontendUrl;
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

             var serializedUser = JsonSerializer.Serialize(result.Value,
                 new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
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

         [HttpGet("qatar-pass/external-login-callback", Name = nameof(QatarPassExternalLoginCallBack))]
         public async Task<IActionResult> QatarPassExternalLoginCallBack(
             [FromQuery] QatarPassExternalCallbackLoginCommand command,
             [FromServices] IOptions<AppConfigSettings> appConfig)
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

             var serializedUser = JsonSerializer.Serialize(result.Value,
                 new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
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
}
