using CSharpFunctionalExtensions;
using MediatR;

namespace Tawtheef.Application.Features.Authenticator.Commands;

public record ValidateLinkTokenCommand(string Token) : IRequest<Result<LinkTokenValidationResult>>;