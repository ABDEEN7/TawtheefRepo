using CSharpFunctionalExtensions;
using MediatR;
using Tawtheef.Application.Features.Authenticator.DTOs.Responses;

namespace Tawtheef.Application.Features.Authenticator.Commands;

public sealed record AzureExternalCallbackLoginCommand(
    string? IdToken = null,
    string? AccessToken = null,
    string? ReturnUrl = null,
    string? Error = null
) : IRequest<Result<AuthResponse>>;
