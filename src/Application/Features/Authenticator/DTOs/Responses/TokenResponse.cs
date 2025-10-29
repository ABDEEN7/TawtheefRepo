using System;

namespace Tawtheef.Application.Features.Authenticator.DTOs.Responses;

public record TokenResponse(
    string AccessToken,
    DateTimeOffset AccessTokenExpires,
    string RefreshToken,
    DateTimeOffset RefreshTokenExpires
);
