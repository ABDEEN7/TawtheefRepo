using FluentResults;
using MediatR;
using Tawtheef.Application.Features.Authenticator.DTOs.Responses;

namespace Tawtheef.Application.Features.Authenticator.Commands;

public record QatarPassExternalCallbackLoginCommand(string Authtoken,string? ReturnUrl = null, string? RemoteError = null) : IRequest<IResult<AuthResponse>>;
