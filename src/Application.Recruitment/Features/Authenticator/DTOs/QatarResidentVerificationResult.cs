using System.Text.Json.Serialization;

namespace Application.Recruitment.Features.Authenticator.DTOs;

public sealed record QatarResidentVerificationResult
{
    public string Qid { get; init; } = string.Empty;
    public string PhoneE164 { get; init; } = string.Empty;
}

public class MOIAuthResponse
{
    [JsonPropertyName("access_token")]
    public required string AccessToken { get; init; }
    [JsonPropertyName("token_type")]
    public required string TokenType { get; init; }
    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; init; }
    [JsonPropertyName("userName")]
    public required string UserName { get; init; }
    [JsonPropertyName(".issued")]
    public required string Issued { get; init; }
    [JsonPropertyName(".expires")]
    public required string Expires { get; init; }
}

public class ValidateResponse
{
    [JsonPropertyName("validateCustomerResponse")]
    public ValidateCustomerResponse? ValidateCustomerResponse { get; set; }
}

public class ValidateCustomerResponse
{
    [JsonPropertyName("result")]
    public ResponseData? Result { get; set; }
}

public class ResponseData
{
    [JsonPropertyName("statusCode")]
    public string? StatusCode { get; set; }
    [JsonPropertyName("statusMessage")]
    public string? StatusMessage { get; set; }
}
