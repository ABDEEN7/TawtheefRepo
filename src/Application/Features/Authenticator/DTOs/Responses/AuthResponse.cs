namespace Tawtheef.Application.Features.Authenticator.DTOs.Responses;

public record AuthResponse(
    UserInfoResponse? User = null,
    TokenResponse? Token = null,
    bool RequiresEmailVerification = false,
    UnverifiedEmailData? UnverifiedEmail = null
);

public record UnverifiedEmailData(
    string Email,
    bool CanResend,
    int ResendCooldown
);
