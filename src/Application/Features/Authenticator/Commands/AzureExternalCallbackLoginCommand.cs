using FluentResults;
using MediatR;
using Tawtheef.Application.Features.Authenticator.DTOs.Responses;

namespace Tawtheef.Application.Features.Authenticator.Commands;

public sealed record AzureExternalCallbackLoginCommand(string? ReturnUrl = null,string? Error = null ) : IRequest<IResult<AuthResponse>>;
