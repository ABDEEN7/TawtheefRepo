using System.Text.Json.Serialization;

namespace Tawtheef.Application.Features.Authenticator.DTOs;

public sealed class QatarPassEnvelope
{
    [JsonPropertyName("IsSuccess")] public bool IsSuccess { get; set; }
    [JsonPropertyName("Message")] public string? Message { get; set; }
    [JsonPropertyName("ResponseData")] public List<QatarPassAccount>? ResponseData { get; set; }
}
public sealed class QatarPassAccount
{
    [JsonPropertyName("UserQid")] public string UserQid { get; set; } = default!;
    [JsonPropertyName("MobileNumber")] public string? MobileNumber { get; set; }
    [JsonPropertyName("AccountType")] public string? AccountType { get; set; }
    [JsonPropertyName("AccountSubType")] public string? AccountSubType { get; set; }
    [JsonPropertyName("Code")] public string? Code { get; set; }
    [JsonPropertyName("Nationality")] public string? Nationality { get; set; }
    [JsonPropertyName("AccessTokenExpiration")] public string? AccessTokenExpiration { get; set; }
}
