using CSharpFunctionalExtensions;
using MediatR;

namespace Tawtheef.Application.Features.Authenticator.Commands;

public record RevokeTokenCommand : IRequest<Result<Unit>>
{
    public required string RefreshToken { get; init; }
    public required string IpAddress { get; init; }
}