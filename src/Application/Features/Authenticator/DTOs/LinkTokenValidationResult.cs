namespace Tawtheef.Application.Features.Authenticator.DTOs;

public record LinkTokenValidationResult(Guid UserId, DateTime Expiration);
