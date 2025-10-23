using CSharpFunctionalExtensions;
using MediatR;
using Tawtheef.Application.Features.Authenticator.Commands;
using Tawtheef.Application.Features.Authenticator.DTOs.Responses;

namespace Tawtheef.Application.Features.Authenticator.Handlers.Commands;

public class ExternalLoginWithTokenHandler : IRequestHandler<ExternalLoginWithTokenCommand, Result<AuthResponse>>
{
    public Task<Result<AuthResponse>> Handle(ExternalLoginWithTokenCommand req, CancellationToken ct)
    {
        // 1. find by (Provider, ProviderKey)
        // 2. if not found, optionally create user, set email confirmed etc.
        // 3. create refresh token, create access token, return AuthResponse
        
        throw new NotImplementedException();
    }
}
