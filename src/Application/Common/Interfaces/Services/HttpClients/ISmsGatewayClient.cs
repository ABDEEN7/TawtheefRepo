using CSharpFunctionalExtensions;

namespace Tawtheef.Application.Common.Interfaces.Services.HttpClients;


public interface ISmsGatewayClient
{
    Task<Result<string>> SmsPushAsync(string mobile, string message, CancellationToken ct);
    Task<Result<string>> GenerateOtpAsync(string mobile, CancellationToken ct);
    Task<Result> ValidateOtpAsync(string otpValue, string otpReference, CancellationToken ct);
}
