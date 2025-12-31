namespace Tawtheef.Application.Features.Authenticator.DTOs;

public sealed record QatarResidentVerificationResult
{
    public string Qid { get; init; } = string.Empty;
    public string PhoneE164 { get; init; } = string.Empty;
}
