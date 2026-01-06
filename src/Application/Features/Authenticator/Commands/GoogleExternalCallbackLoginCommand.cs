using Cortex.Mediator.Commands;
using FluentResults;
using Tawtheef.Application.Features.Authenticator.DTOs.Responses;

namespace Tawtheef.Application.Features.Authenticator.Commands;

public record RequestGoogleExternalCallbackLoginCommand(string? ReturnUrl = null, string? RemoteError = null);
public record GoogleExternalCallbackLoginCommand(Guid DefaultUserType, string? ReturnUrl = null, string? RemoteError = null) : ICommand<IResult<AuthResponse>>;
