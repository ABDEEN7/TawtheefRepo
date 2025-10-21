using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Tawtheef.Application.Features.Authenticator.Commands;
using Tawtheef.Application.Features.Authenticator.Queries;
using Tawtheef.Domain.Configurations;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Users;
using Tawtheef.Infrastructure.Extensions;

namespace Tawtheef.Web.Controllers;

[ApiController]
[Route("api/[controller]/profile")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class UserController(IMediator mediator) : ControllerBase
{
    private Result<Guid> UserId => User.FindFirst(ClaimTypes.NameIdentifier)?.Value switch
    {
        null => Result.Failure<Guid>(ErrorsCodes.InvalidUserIdentifier),
        var id => Result.Success(Guid.Parse(id))
    };
    
    [HttpGet]
    public async Task<IActionResult> GetProfile()
    {
        if(UserId.IsFailure)
            return Unauthorized(UserId.Error);
        var result = await mediator.Send(new GetUserProfileQuery{ UserId = UserId.Value});
        return result.ToActionResult();
    }
    
    [HttpGet("social-accounts")]
    public async Task<IActionResult> GetSocialAccounts()
    {
        if(UserId.IsFailure)
            return Unauthorized(UserId.Error);
        var result = await mediator.Send(new GetUserSocialAccountsQuery(UserId.Value));
        return result.ToActionResult();
    }
    [HttpGet("external-link/init")]
    public async Task<IActionResult> InitiateExternalLink(
        [AllowedValues("Google", "Outlook", ErrorMessage = "Invalid provider. Supported providers: Google, Outlook")] 
        [FromQuery] string provider)
    {
        if (UserId.IsFailure)
            return BadRequest(UserId.Error);

        if (string.IsNullOrEmpty(provider))
            return BadRequest("Provider is required");

        // Generate a short-lived token (valid for 2 minutes)
        var token = await mediator.Send(new GenerateLinkTokenCommand(UserId.Value));
    
        return Ok(new { token });
    }

    [AllowAnonymous]
    [HttpGet("external-link")]
    public async Task<IActionResult> LinkExternalLogin([FromQuery] ExternalLinkCommand command, [FromServices] SignInManager<User> signInManager)
    {
        // Validate the short-lived token
        var tokenValidation = await mediator.Send(new ValidateLinkTokenCommand(command.Token));
        if (tokenValidation.IsFailure)
            return BadRequest(tokenValidation.Error);

        if (string.IsNullOrEmpty(command.Provider))
            return BadRequest("Provider is required");

        var returnUrl = command.ReturnUrl ?? Url.Content("~/");
        var redirectUrl = Url.Action("ExternalLinkCallback", "User", new { returnUrl, token = command.Token });

        var properties = signInManager.ConfigureExternalAuthenticationProperties(
            command.Provider,
            redirectUrl,
            tokenValidation.Value.UserId.ToString());

        return Challenge(properties, command.Provider);
    }
    
    [AllowAnonymous]
    [HttpGet("ExternalLinkCallback")]
    public async Task<IActionResult> ExternalLinkCallback([FromQuery] ExternalCallbackLinkCommand command, [FromServices] IOptions<AppConfigSettings> appConfig)
    {
        // Validate the short-lived token
        var tokenValidation = await mediator.Send(new ValidateLinkTokenCommand(command.Token));
        if (tokenValidation.IsFailure)
            return BadRequest(tokenValidation.Error);
        
        //extract userId from the token
        var userId = tokenValidation.Value.UserId;
        if (userId == Guid.Empty)
            return BadRequest("Invalid user ID in token");
        
        var frontEndOrigin = appConfig.Value.FrontendUrl;
        var result = await mediator.Send(command with { UserId = userId });
        
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
                                        type: 'EXTERNAL_LINK_ERROR',
                                        provider: '{{result.Value.Provider}}',
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
                                 type: 'EXTERNAL_LINK_SUCCESS',
                                 provider: '{{result.Value.Provider}}',
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

    [HttpDelete("unlink-social/{provider}")]
    public async Task<IActionResult> UnlinkSocialAccount(
        [AllowedValues("Google", "Outlook", ErrorMessage = "Invalid provider. Supported providers: Google, Outlook")] 
        string provider)
    {
        if (UserId.IsFailure)
            return Unauthorized(UserId.Error);
        var result = await mediator.Send(new UnlinkSocialAccountCommand(UserId.Value, provider));
        return result.ToActionResult();
    }
}
