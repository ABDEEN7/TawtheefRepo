using System.Net.Http.Headers;
using System.Runtime.Serialization;
using FluentResults;
using Microsoft.Extensions.Options;
using Tawtheef.Application.Common.Interfaces.Services.HttpClients;
using Tawtheef.Application.Features.Recruitment.Profile.DTOs;
using Tawtheef.Domain.Configurations.Settings;

namespace Tawtheef.Infrastructure.Services.HttpClients;


public sealed class MoiClient : IMoiClient
{
    private readonly HttpClient _http;

    public MoiClient(HttpClient http, IOptions<MoiSettings> opt)
    {
        _http = http;
        _http.BaseAddress ??= new Uri(opt.Value.BaseUrl);
        _http.Timeout = TimeSpan.FromSeconds(10);
    }

    public async Task<IResult<MOEPersonalInfo>> GetPersonalInfoAsync(
        string qid,
        DateOnly expiryDate,
        CancellationToken ct = default)
    {
        try
        {
            var url = $"GetPersonalInfo?Qid={qid}&ExpiryDate={expiryDate:yyyy-MM-dd}";
            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
            using var response = await _http.SendAsync(request, ct);
            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync(ct);
                return Result.Fail<MOEPersonalInfo>(
                    $"MOI API error: {(int)response.StatusCode} {response.ReasonPhrase}. Body: {body}");
            }

            await using var stream = await response.Content.ReadAsStreamAsync(ct);

            var serializer = new DataContractSerializer(typeof(PersonalInfoApiResponse));
            if (serializer.ReadObject(stream) is not PersonalInfoApiResponse apiResponse)
                return Result.Fail<MOEPersonalInfo>("Failed to deserialize MOI response");

            if (!apiResponse.IsSuccess)
                return Result.Fail<MOEPersonalInfo>(apiResponse.Message ?? "MOI API returned failure");

            if (apiResponse.ResponseData is null)
                return Result.Fail<MOEPersonalInfo>("MOI API returned no data");

            return Result.Ok(apiResponse.ResponseData);
        }
        catch (Exception ex)
        {
            return Result.Fail<MOEPersonalInfo>($"Exception calling MOI API: {ex.Message}");
        }
    }
}

