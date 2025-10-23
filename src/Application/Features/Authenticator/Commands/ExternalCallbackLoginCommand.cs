using CSharpFunctionalExtensions;
using MediatR;
using Tawtheef.Application.Features.Authenticator.DTOs.Responses;

namespace Tawtheef.Application.Features.Authenticator.Commands;

public record ExternalCallbackLoginCommand(string? ReturnUrl = null, string? RemoteError = null) : IRequest<Result<LoginResponse>>;