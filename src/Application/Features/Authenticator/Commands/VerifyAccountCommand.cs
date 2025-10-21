using CSharpFunctionalExtensions;
using MediatR;

namespace Tawtheef.Application.Features.Authenticator.Commands;

public record VerifyAccountCommand : IRequest<Result<bool>>
{
    public required string Token { get; init; }
    public required string Email { get; init; }
}