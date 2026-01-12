using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FluentResults;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Domain.Configurations.Settings;
using Tawtheef.Domain.Constants;

namespace Tawtheef.Infrastructure.Services.Identity;


public sealed class AzureIdTokenValidator(
    ILogger logger,
    IOptions<AzureAuthenticationSettings> azureOptions,
    IConfigurationManager<OpenIdConnectConfiguration> oidcManager)
    : IExternalIdTokenValidator
{
    public async Task<IResult<ClaimsPrincipal>> ValidateAsync(string idToken, CancellationToken ct)
    {
        try
        {
            var azure = azureOptions.Value;
            var oidc = await oidcManager.GetConfigurationAsync(ct);
            var parameters = new TokenValidationParameters{
                // ===== Always enforce security invariants =====
                RequireSignedTokens = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKeys = oidc.SigningKeys,

                ValidateAudience = true,
                ValidAudience = azure.ClientId,

                ValidateLifetime = true,
                ClockSkew = azure.ClockSkew,

                ValidateIssuer = true,
                IssuerValidator = AzureIssuerValidator,

                NameClaimType = "name",
                RoleClaimType = "roles"
            };

            var handler = new JwtSecurityTokenHandler();
            var principal = handler.ValidateToken(idToken, parameters, out _);

            return Result.Ok(principal);
        }
        catch(Exception ex)
        {
            logger.Error(ex, "Azure IdToken validation failed");
            return Result.Fail<ClaimsPrincipal>(ErrorsCodes.ExternalLoginInvalidToken);
        }
    }

    private static string AzureIssuerValidator(string issuer, SecurityToken token, TokenValidationParameters _)
    {
        if (token is not JwtSecurityToken jwt)
            throw new SecurityTokenInvalidIssuerException($"Unexpected issuer: {issuer}");

        var tid = jwt.Claims.FirstOrDefault(c => c.Type == "tid")?.Value;
        if (string.IsNullOrWhiteSpace(tid))
            throw new SecurityTokenInvalidIssuerException($"Missing tid. Issuer: {issuer}");

        var v2 = $"https://login.microsoftonline.com/{tid}/v2.0";
        var sts = $"https://sts.windows.net/{tid}/";

        if (issuer.Equals(v2, StringComparison.OrdinalIgnoreCase) ||
            issuer.Equals(sts, StringComparison.OrdinalIgnoreCase))
            return issuer;

        throw new SecurityTokenInvalidIssuerException($"Unexpected issuer: {issuer}");
    }
}
