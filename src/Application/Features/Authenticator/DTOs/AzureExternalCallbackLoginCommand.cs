using CSharpFunctionalExtensions;
using MediatR;
using Tawtheef.Application.Features.Authenticator.DTOs.Responses;

namespace Tawtheef.Application.Features.Authenticator.DTOs;

public sealed record AzureExternalCallbackLoginCommand(
    string Provider,
    string IdToken,
    string? ClientIp = null,
    string? UserAgent = null,
    string? RemoteError = null
) : IRequest<Result<AuthResponse>>;
