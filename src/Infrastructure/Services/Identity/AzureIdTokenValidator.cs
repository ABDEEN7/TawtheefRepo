using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FluentResults;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Domain.Constants;

namespace Tawtheef.Infrastructure.Services.Identity;


public sealed class AzureIdTokenValidator(IConfiguration config) : IExternalIdTokenValidator
{
    public async Task<IResult<ClaimsPrincipal>> ValidateAsync(string idToken, CancellationToken ct)
    {
        try
        {
            var tenant = config["Authentication:Azure:TenantId"] ?? "common";
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
                IssuerValidator = (issuer, securityToken, validationParameters) =>
                {
                    if (securityToken is JwtSecurityToken jwt)
                    {
                        var tid = jwt.Claims.FirstOrDefault(c => c.Type == "tid")?.Value;
                        if (!string.IsNullOrEmpty(tid))
                        {
                            var v2 = $"https://login.microsoftonline.com/{tid}/v2.0";
                            var sts = $"https://sts.windows.net/{tid}/";
                            if (issuer.Equals(v2, StringComparison.OrdinalIgnoreCase) ||
                                issuer.Equals(sts, StringComparison.OrdinalIgnoreCase))
                                return issuer;
                        }
                    }
                    throw new SecurityTokenInvalidIssuerException($"Unexpected issuer: {issuer}");
                },
                ValidateIssuerSigningKey = true,
                IssuerSigningKeys = oidc.SigningKeys,
                ValidateAudience = true,
                ValidAudiences = [ config["Authentication:Azure:ClientId"]! ],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(2),
                NameClaimType = "name",
                RoleClaimType = "roles"
            };

            var handler = new JwtSecurityTokenHandler();
            var principal = handler.ValidateToken(idToken, parameters, out _);
            return Result.Ok(principal);
        }
        catch
        {
            return Result.Fail<ClaimsPrincipal>(ErrorsCodes.ExternalLoginInvalidToken);
        }
    }
}
