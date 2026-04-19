using System.Net.Http.Headers;
using System.Runtime.Serialization;
using System.Xml;
using FluentResults;
using Tawtheef.Application.Common.Interfaces.Logging;
using Tawtheef.Application.Common.Interfaces.Services.HttpClients;
using Tawtheef.Application.Features.Authenticator.DTOs;
using Tawtheef.Domain.Constants;

namespace Tawtheef.Infrastructure.Services.HttpClients;


public sealed class MoiClient(IAppLogger logger, HttpClient http): IMoiClient
{
    public async Task<IResult<MOEPersonalInfo>> GetPersonalInfoAsync(
        string qid, DateOnly expiryDate, CancellationToken ct = default)
    {
        var url = $"API/MOIApi/V1/GetPersonalInfo?Qid={qid}&ExpiryDate={expiryDate:yyyy-MM-dd}";

        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));

            using var response = await http.SendAsync(request, ct);

            var body = await response.Content.ReadAsStringAsync(ct); // read ONCE

            if (!response.IsSuccessStatusCode)
            {
                logger.Error($"MOI API error: {(int)response.StatusCode} {response.ReasonPhrase}. Body: {body}");
                return Result.Fail<MOEPersonalInfo>(ErrorsCodes.MOIFailedRequest);
            }

            PersonalInfoApiResponse? apiResponse;
            try
            {
                var serializer = new DataContractSerializer(typeof(PersonalInfoApiResponse));

                using var sr = new StringReader(body);
                using var xr = XmlReader.Create(sr);

                apiResponse = serializer.ReadObject(xr) as PersonalInfoApiResponse;
            }
            catch (SerializationException ex)
            {
                logger.Error(ex, $"Failed to deserialize MOI API response. Body: {body}");
                return Result.Fail<MOEPersonalInfo>(ErrorsCodes.MOIFailedRequest);
            }

            if (apiResponse is null)
            {
                logger.Error($"Failed to deserialize MOI API response (null result). Body: {body}");
                return Result.Fail<MOEPersonalInfo>(ErrorsCodes.MOIFailedRequest);
            }

            if (!apiResponse.IsSuccess)
            {
                if (apiResponse.Message?.Contains("Data not found", StringComparison.OrdinalIgnoreCase) == true)
                    return Result.Fail<MOEPersonalInfo>(ErrorsCodes.MOIDataNotFound);

                logger.Error($"MOI API returned failure: {apiResponse.Message}. Body: {body}");
                return Result.Fail<MOEPersonalInfo>(ErrorsCodes.MOIFailedRequest);
            }

            if (apiResponse.ResponseData is null)
            {
                logger.Error($"MOI API returned null response data. Body: {body}");
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

