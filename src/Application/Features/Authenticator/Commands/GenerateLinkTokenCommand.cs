using System;
using MediatR;

namespace Tawtheef.Application.Features.Authenticator.Commands;

public record GenerateLinkTokenCommand(Guid UserId) : IRequest<string>;