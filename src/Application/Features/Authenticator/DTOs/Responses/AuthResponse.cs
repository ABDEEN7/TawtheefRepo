namespace Tawtheef.Application.Features.Authenticator.DTOs.Responses;

public record AuthResponse(
    bool RequiresProfileCompletion,
    UserInfoResponse? User = null,
    TokenResponse? Token = null,
    string[]? MissingFields = null,
    ProfilePrefillDto? Prefill = null
);

public sealed class ProfilePrefillDto
{
    public string? Email { get; init; }
    public bool EmailVerified { get; init; }
    public string? FullName { get; init; }
    public string? Avatar { get; init; }
    public string? Phone { get; init; }
    public bool PhoneVerified { get; init; }
    public string? Nationality { get; init; }
    public string? Qid { get; init; }
    public string? Locale { get; init; }
    public string? Provider { get; init; }
}
