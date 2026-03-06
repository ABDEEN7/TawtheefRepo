using MediatR;
using FluentResults;
using Tawtheef.Application.Features.Authenticator.DTOs.Responses;

namespace Tawtheef.Application.Features.Authenticator.Commands;

public record RefreshTokenCommand : IRequest<IResult<TokenResponse>>
{
    public required string RefreshToken { get; init; }
    public string? IpAddress { get; init; }
}

