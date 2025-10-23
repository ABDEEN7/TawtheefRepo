using System;

namespace Tawtheef.Application.Features.Authenticator.DTOs.Responses;

public record TokenResponse(
    string AccessToken,
    DateTime AccessTokenExpires,
    string RefreshToken,
    DateTime RefreshTokenExpires
);