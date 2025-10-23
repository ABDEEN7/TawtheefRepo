using CSharpFunctionalExtensions;
using MediatR;

namespace Tawtheef.Application.Features.Authenticator.Commands;

public record VerifyEmailViaSmsCommand : IRequest<Result>
{
    public required string Email { get; set; }
    public required string PhoneNumber { get; set; }
}