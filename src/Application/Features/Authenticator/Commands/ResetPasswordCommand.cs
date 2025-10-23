using CSharpFunctionalExtensions;
using MediatR;

namespace Tawtheef.Application.Features.Authenticator.Commands;

public record ResetPasswordCommand(string Email, string Token, string NewPassword) : IRequest<Result<Unit>>;