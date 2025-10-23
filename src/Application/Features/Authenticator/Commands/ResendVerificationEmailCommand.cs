using CSharpFunctionalExtensions;
using MediatR;
using Tawtheef.Application.Features.Authenticator.DTOs.Responses;

namespace Tawtheef.Application.Features.Authenticator.Commands;

public record ResendVerificationEmailCommand : IRequest<Result<UnverifiedEmailData>>
{
    public required string Email { get; set; }
    public string? RecipientName { get; set; }
}