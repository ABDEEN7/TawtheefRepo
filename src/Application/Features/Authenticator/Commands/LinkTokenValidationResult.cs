using System;

namespace Tawtheef.Application.Features.Authenticator.Commands;

public record LinkTokenValidationResult(Guid UserId, DateTime Expiration);