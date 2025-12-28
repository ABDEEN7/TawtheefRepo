using FluentResults;
using MediatR;
using Tawtheef.Application.Features.Authenticator.DTOs.Responses;

namespace Tawtheef.Application.Features.Authenticator.Commands;

public record RequestGoogleExternalCallbackLoginCommand(string? ReturnUrl = null, string? RemoteError = null);
public record GoogleExternalCallbackLoginCommand(Guid DefaultUserType, string? ReturnUrl = null, string? RemoteError = null) : IRequest<IResult<AuthResponse>>;
