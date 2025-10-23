using System;
using CSharpFunctionalExtensions;
using MediatR;

namespace Tawtheef.Application.Features.Authenticator.Commands;

public record VerifyLockPasswordCommand(Guid UserId, string Password) : IRequest<Result<bool>>;