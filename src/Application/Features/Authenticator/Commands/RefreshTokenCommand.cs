using CSharpFunctionalExtensions;
using MediatR;
using Tawtheef.Application.Features.Authenticator.DTOs.Responses;

namespace Tawtheef.Application.Features.Authenticator.Commands;

public record RefreshTokenCommand : IRequest<Result<TokenResponse>>
{
    public required string AccessToken { get; init; }
    public required string RefreshToken { get; init; }
    public string? IpAddress { get; init; }
}