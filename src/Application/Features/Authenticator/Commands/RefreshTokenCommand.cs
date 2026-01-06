using Cortex.Mediator.Commands;
using FluentResults;
using Tawtheef.Application.Features.Authenticator.DTOs.Responses;

namespace Tawtheef.Application.Features.Authenticator.Commands;

public record RefreshTokenCommand : ICommand<IResult<TokenResponse>>
{
    public required string AccessToken { get; init; }
    public required string RefreshToken { get; init; }
    public string? IpAddress { get; init; }
}
