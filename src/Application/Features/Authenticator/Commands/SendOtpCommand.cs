using CSharpFunctionalExtensions;
using MediatR;

namespace Tawtheef.Application.Features.Authenticator.Commands;

public record SendOtpCommand(string Email) : IRequest<Result>;
