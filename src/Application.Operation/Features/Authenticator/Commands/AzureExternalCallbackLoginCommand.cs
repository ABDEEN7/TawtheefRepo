using MediatR;
using FluentResults;
using Tawtheef.Application.Features.Authenticator.DTOs.Responses;

namespace Application.Operation.Features.Authenticator.Commands;

public sealed record AzureExternalCallbackLoginCommand(string? ReturnUrl = null,string? Error = null ) 
    : IRequest<IResult<AuthResponse>>;

