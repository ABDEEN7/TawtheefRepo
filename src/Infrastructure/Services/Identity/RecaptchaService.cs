using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using Tawtheef.Application.Common.Interfaces.Services.Security;
using Tawtheef.Application.Common.Models;
using Tawtheef.Domain.Configurations.Settings;

namespace Tawtheef.Infrastructure.Services.Identity;

public sealed class RecaptchaService(HttpClient httpClient, IOptions<RecaptchaSettings> recaptchaSettings)
    : IRecaptchaService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private readonly string _secretKey = recaptchaSettings.Value.SecretKey;

    public async Task<RecaptchaResponse> VerifyAsync(string recaptchaToken, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(recaptchaToken))
            return new RecaptchaResponse { Success = false };

        var values = new Dictionary<string, string>
        {
            ["secret"]   = _secretKey,
            ["response"] = recaptchaToken
        };

        using var content = new FormUrlEncodedContent(values);

        HttpResponseMessage response;
        try
        {
            response = await httpClient.PostAsync("siteverify", content, ct);
        }
        catch (Exception ex) when (ex is TaskCanceledException or HttpRequestException)
        {
            // You can inject a logger here if you want
            return new RecaptchaResponse { Success = false, ErrorCodes = [ex.Message] };
        }

        if (!response.IsSuccessStatusCode)
            return new RecaptchaResponse { Success = false };

        var responseString = await response.Content.ReadAsStringAsync(ct);

        var result = JsonSerializer.Deserialize<RecaptchaResponse>(responseString, JsonOptions);

        return result ?? new RecaptchaResponse { Success = false };
    }
}
