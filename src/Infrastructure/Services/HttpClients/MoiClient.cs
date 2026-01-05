using System.Net.Http.Headers;
using System.Runtime.Serialization;
using FluentResults;
using Serilog;
using Tawtheef.Application.Common.Interfaces.Services.HttpClients;
using Tawtheef.Application.Features.Recruitment.Profile.DTOs;
using Tawtheef.Domain.Constants;

namespace Tawtheef.Infrastructure.Services.HttpClients;


public sealed class MoiClient(ILogger logger, HttpClient http): IMoiClient
{
    public async Task<IResult<MOEPersonalInfo>> GetPersonalInfoAsync(
        string qid, DateOnly expiryDate, CancellationToken ct = default)
    {
        try
        {
            var url = $"API/MOIApi/V1/GetPersonalInfo?Qid={qid}&ExpiryDate={expiryDate:yyyy-MM-dd}";
            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
            using var response = await http.SendAsync(request, ct);
            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync(ct);
                logger.Error($"MOI API error: {(int)response.StatusCode} {response.ReasonPhrase}. Body: {body}");
                return Result.Fail<MOEPersonalInfo>(ErrorsCodes.MOIFailedRequest);
            }

            await using var stream = await response.Content.ReadAsStreamAsync(ct);

            var serializer = new DataContractSerializer(typeof(PersonalInfoApiResponse));
            if (serializer.ReadObject(stream) is not PersonalInfoApiResponse apiResponse)
            {
                var body = await response.Content.ReadAsStringAsync(ct);
                logger.Error($"Failed to deserialize MOI API response. Body: {body}");
                return Result.Fail<MOEPersonalInfo>(ErrorsCodes.MOIFailedRequest);
            }

            if (!apiResponse.IsSuccess)
            {
                if (apiResponse.Message?.Contains("Data not found") ?? false)
                {
                    return Result.Fail<MOEPersonalInfo>(ErrorsCodes.MOIDataNotFound);
                }
                logger.Error("MOI API returned failure: {Message}", apiResponse.Message);
                return Result.Fail<MOEPersonalInfo>(ErrorsCodes.MOIFailedRequest);
            }

            if (apiResponse.ResponseData is null)
            {
                logger.Error("MOI API returned null response data");
                return Result.Fail<MOEPersonalInfo>(ErrorsCodes.MOIFailedRequest);
            }

            return Result.Ok(apiResponse.ResponseData);
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Exception occurred while calling MOI API");
            return Result.Fail<MOEPersonalInfo>(ErrorsCodes.MOIFailedRequest);
        }
    }
}

