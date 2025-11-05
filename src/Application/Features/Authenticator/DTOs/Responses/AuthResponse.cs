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
    public string? GivenNameEn { get; init; }
    public string? FamilyNameEn { get; init; }
    public string? Avatar { get; init; }
    public string? PhoneE164 { get; init; }
    public string? Nationality { get; init; }
    public string? PassportNo { get; init; }
    public string? Qid { get; init; }
    public string? Locale { get; init; }
    public string? Provider { get; init; }
}
