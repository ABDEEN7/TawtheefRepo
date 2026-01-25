using System.Text;
using Microsoft.IdentityModel.Tokens;
using Tawtheef.Domain.Configurations.Settings;

namespace Tawtheef.Application.Common.Validations;

public static class JwtValidationMapper
{
    public static TokenValidationParameters ToTokenValidationParameters(
        this JwtSettings settings, string issuer, string audience)
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(settings.SigningKey));

        return new TokenValidationParameters
        {
            // ===== Always enforce cryptography =====
            IssuerSigningKey = key,
            RequireSignedTokens = true,
            RequireExpirationTime = true,

            // ===== Identity =====
            ValidIssuer = issuer,
            ValidAudience = audience,

            // ===== Controlled flags from config =====
            ValidateIssuer = settings.TokenValidationParameters.ValidateIssuer,
            ValidateAudience = settings.TokenValidationParameters.ValidateAudience,
            ValidateLifetime = settings.TokenValidationParameters.ValidateLifetime,
            ValidateIssuerSigningKey = true, // NEVER allow config to disable this

            ClockSkew = settings.TokenValidationParameters.ClockSkew
        };
    }
}
