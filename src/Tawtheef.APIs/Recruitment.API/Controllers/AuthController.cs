using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using CSharpFunctionalExtensions;
using Mapster;
using MapsterMapper;
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
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Configurations;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;
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

        [HttpGet("external-login")]
        public IActionResult ExternalLogin([FromQuery] ExternalLoginRequest request, [FromServices] SignInManager<User> signInManager)
        {
            var provider = request.Provider;
            if (string.IsNullOrEmpty(provider))
                return BadRequest(ErrorsCodes.ExternalLoginProviderRequired);
        
            var returnUrl = request.ReturnUrl ?? Url.Content("~/");
            var redirectUrl = Url.ActionLink(nameof(ExternalLoginCallback), controller: null, values: new { returnUrl }, protocol: Request.Scheme);
            var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        [HttpPost("external-login/token")]
        public async Task<IActionResult> ExternalLoginWithToken([FromBody] ExternalTokenDto dto)
        {
            if (string.IsNullOrEmpty(dto.Provider) || string.IsNullOrEmpty(dto.IdToken))
                return BadRequest(ErrorsCodes.InvalidRequest);

            var provider = dto.Provider.Trim().ToLowerInvariant();
            ClaimsPrincipal principal;

            try
            {
                if (provider == "azure" || provider == "azuread" || provider == "microsoft")
                {
                    principal = await ValidateAzureIdToken(dto.IdToken, HttpContext.RequestServices);
                }
                else
                {
                    return BadRequest(ErrorsCodes.ExternalLoginProviderNotSupported);
                }
            }
            catch (Exception)
            {
                // log ex if you have logging
                return BadRequest(ErrorsCodes.ExternalLoginInvalidToken);
            }

            // Extract standard claims (adjust names as needed)
            var providerKey = principal.FindFirst("sub")?.Value ?? principal.FindFirst("oid")?.Value;
            var email = principal.FindFirst(ClaimTypes.Email)?.Value ??
                        principal.FindFirst("preferred_username")?.Value;
            var name = principal.FindFirst(ClaimTypes.Name)?.Value ?? principal.FindFirst("name")?.Value;

            if (string.IsNullOrEmpty(providerKey))
                return BadRequest(ErrorsCodes.ExternalLoginMissingProviderKey);

            // Convert claims to simple KVP
            var claims = principal.Claims.Select(c => new KeyValuePair<string, string>(c.Type, c.Value));

            // Build MediatR command - adapt to your existing command/response types
            var cmd = new ExternalLoginWithTokenCommand(
                Provider: dto.Provider,
                ProviderKey: providerKey,
                Email: email ?? string.Empty,
                DisplayName: name ?? string.Empty,
                Claims: claims,
                RawIdToken: dto.IdToken,
                ClientIp: HttpContext.GetClientIpAddress() ?? "Unknown IP Address"
            );

            var result = await mediator.Send(cmd);

            return result.ToActionResult();
            async Task<ClaimsPrincipal> ValidateAzureIdToken(string idToken, IServiceProvider services)
            {
                var config = services.GetService<IConfiguration>();
                // You may want to use a specific tenant id or "common" depending on your setup
                var tenant = config!["AzureAd:TenantId"] ?? "common";
                // Use v2.0 endpoint for tokens
                var authority = $"https://login.microsoftonline.com/{tenant}/v2.0";
                var metadataAddress = $"{authority}/.well-known/openid-configuration";

                var documentRetriever = new Microsoft.IdentityModel.Protocols.HttpDocumentRetriever { RequireHttps = true };
                var configManager = new Microsoft.IdentityModel.Protocols.ConfigurationManager<Microsoft.IdentityModel.Protocols.OpenIdConnect.OpenIdConnectConfiguration>(
                    metadataAddress,
                    new Microsoft.IdentityModel.Protocols.OpenIdConnect.OpenIdConnectConfigurationRetriever(),
                    documentRetriever);

                var openIdConfig = await configManager.GetConfigurationAsync();

                var validationParameters = new TokenValidationParameters
                {
                    ValidIssuers = ["https://login.microsoftonline.com/" + tenant + "/v2.0", "https://sts.windows.net/" + tenant + "/"
                    ],
                    ValidateIssuer = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKeys = openIdConfig.SigningKeys,
                    ValidateAudience = true,
                    ValidAudiences =
                    [
                        config["AzureAd:ClientId"]  // the client id of your SPA or API depending on which token you expect
                    ],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(2)
                };

                var handler = new JwtSecurityTokenHandler();
                var principal = handler.ValidateToken(idToken, validationParameters, out var validatedToken);

                return principal;
            }
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
}
