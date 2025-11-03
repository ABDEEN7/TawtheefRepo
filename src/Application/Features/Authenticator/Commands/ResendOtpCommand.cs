using CSharpFunctionalExtensions;
using MediatR;

namespace Tawtheef.Application.Features.Authenticator.Commands;

public record ResendOtpCommand(string Email) : IRequest<Result>;
