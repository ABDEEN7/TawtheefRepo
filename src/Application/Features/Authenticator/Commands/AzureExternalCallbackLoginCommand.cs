using Cortex.Mediator.Commands;
using FluentResults;
using Tawtheef.Application.Features.Authenticator.DTOs.Responses;

namespace Tawtheef.Application.Features.Authenticator.Commands;

public sealed record AzureExternalCallbackLoginCommand(string? ReturnUrl = null,string? Error = null ) : ICommand<IResult<AuthResponse>>;
