using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Common.Models;
using Tawtheef.Domain.Configurations;
using Tawtheef.Domain.Configurations.Settings;

namespace Tawtheef.Infrastructure.Services;

public class RecaptchaService(HttpClient httpClient, IOptions<RecaptchaSettings> recaptchaSettings) : IRecaptchaService
{
    private readonly string _secretKey = recaptchaSettings.Value.SecretKey;

    public async Task<RecaptchaResponse> Verify(string recaptchaToken)
    {
        var values = new Dictionary<string, string>
        {
            { "secret", _secretKey },
            { "response", recaptchaToken }
        };

        var content = new FormUrlEncodedContent(values);
        var response = await httpClient.PostAsync("https://www.google.com/recaptcha/api/siteverify", content);

        if (!response.IsSuccessStatusCode)
            return new RecaptchaResponse {Success = false};

        var responseString = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<RecaptchaResponse>(responseString, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        });

        return result ?? new RecaptchaResponse {Success = false};
    }
}