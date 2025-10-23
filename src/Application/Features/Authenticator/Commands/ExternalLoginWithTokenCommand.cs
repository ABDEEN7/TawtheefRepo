using System.ComponentModel.DataAnnotations;
using CSharpFunctionalExtensions;
using MediatR;
using Tawtheef.Application.Features.Authenticator.DTOs.Responses;

namespace Tawtheef.Application.Features.Authenticator.Commands;

public record ExternalLoginWithTokenCommand(
    [AllowedValues("Google", "AzureAD", ErrorMessage = "Invalid provider. Supported providers: Google, AzureAD")] 
    string Provider,
    string ProviderKey,
    string Email,
    string DisplayName,
    IEnumerable<KeyValuePair<string,string>> Claims,
    string RawIdToken,
    string ClientIp
) : IRequest<Result<AuthResponse>>;
