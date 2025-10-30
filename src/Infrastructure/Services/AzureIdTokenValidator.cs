using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Domain.Constants;

namespace Tawtheef.Infrastructure.Services;


public sealed class AzureIdTokenValidator(IConfiguration config) : IExternalIdTokenValidator
{
    public async Task<Result<ClaimsPrincipal>> ValidateAsync(string idToken, CancellationToken ct)
    {
        try
        {
            var tenant = config["AzureAd:TenantId"] ?? "common";
            var authority = $"https://login.microsoftonline.com/{tenant}/v2.0";
            var metadataAddress = $"{authority}/.well-known/openid-configuration";

            var retriever = new HttpDocumentRetriever { RequireHttps = true };
            var manager = new ConfigurationManager<OpenIdConnectConfiguration>(
                metadataAddress,
                new OpenIdConnectConfigurationRetriever(),
                retriever);

            var oidc = await manager.GetConfigurationAsync(ct);

            var parameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuers =
                [
                    $"https://login.microsoftonline.com/{tenant}/v2.0",
                    $"https://sts.windows.net/{tenant}/"
                ],
                ValidateIssuerSigningKey = true,
                IssuerSigningKeys = oidc.SigningKeys,
                ValidateAudience = true,
                ValidAudiences = [ config["AzureAd:ClientId"]! ],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(2)
            };

            var handler = new JwtSecurityTokenHandler();
            var principal = handler.ValidateToken(idToken, parameters, out _);
            return Result.Success(principal);
        }
        catch
        {
            return Result.Failure<ClaimsPrincipal>(ErrorsCodes.ExternalLoginInvalidToken);
        }
    }
}
