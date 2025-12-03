using System.Net.Http.Headers;
using System.Runtime.Serialization;
using System.Text;
using FluentResults;
using Microsoft.Extensions.Options;
using Tawtheef.Application.Common.Interfaces.Services.HttpClients;
using Tawtheef.Application.Features.Recruitment.Profile.DTOs;
using Tawtheef.Domain.Configurations.Settings;

namespace Tawtheef.Infrastructure.Services.HttpClients;

public sealed class MoiClient : IMoiClient
{
    private readonly HttpClient _http;
    private readonly MoiSettings _opt;

    public MoiClient(HttpClient http, IOptions<MoiSettings> opt)
    {
        _http = http;
        _opt  = opt.Value;
        _http.BaseAddress ??= new Uri(_opt.BaseUrl);
        // _http.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/xml"));
        var byteArray = Encoding.ASCII.GetBytes($"{_opt.Username}:{_opt.Password}");
        _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
        _http.Timeout = TimeSpan.FromSeconds(10);
    }

    public async Task<IResult<MOEPersonalInfo>> GetPersonalInfoAsync(string qid, DateOnly expiryData, CancellationToken ct = default)
    {
        var url = $"GetPersonalInfo?qid={qid}&ExpiryDate=${expiryData.ToString("yyyy-MM-dd")}";
        using var response = await _http.GetAsync(url, ct);
        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync(ct);
        var serializer = new DataContractSerializer(typeof(PersonalInfoApiResponse));
        if (serializer.ReadObject(stream) is not PersonalInfoApiResponse apiResponse)
            return Result.Fail<MOEPersonalInfo>("Failed to deserialize MOI response");

        if (!apiResponse.IsSuccess)
            return Result.Fail<MOEPersonalInfo>(apiResponse.Message ?? "MOI API returned failure");

        return Result.Ok(apiResponse.ResponseData!);
    }
}
