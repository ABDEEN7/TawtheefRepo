using CSharpFunctionalExtensions;
using MediatR;

namespace Tawtheef.Application.Features.Authenticator.Commands;

public record ForgetPasswordCommand(string Email) : IRequest<Result<Unit>>;