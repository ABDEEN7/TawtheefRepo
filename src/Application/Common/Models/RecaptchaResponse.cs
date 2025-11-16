using System.Text.Json.Serialization;

namespace Tawtheef.Application.Common.Models;

public record RecaptchaResponse
{
    public bool Success { get; init; }
    [JsonPropertyName("challenge_ts")]
    public DateTime ChallengeTs { get; init; }
    public string? Hostname { get; init; }
    public double? Score { get; init; }
    public string? Action { get; init; }
    [JsonPropertyName("error_codes")]
    public List<string>? ErrorCodes { get; init; }
}




