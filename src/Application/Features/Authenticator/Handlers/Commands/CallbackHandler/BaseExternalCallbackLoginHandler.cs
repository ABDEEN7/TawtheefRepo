using FluentResults;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Features.Authenticator.DTOs.Responses;

namespace Tawtheef.Application.Features.Authenticator.Handlers.Commands.CallbackHandler;

public abstract class BaseExternalCallbackLoginHandler(ILoginAuditService loginAudit)
{
    protected abstract string Provider { get; }
    protected abstract Guid? DefaultUserType { get; }
    protected async Task<IResult<AuthResponse>> LogFailureAsync(string reason, 
        Guid? userId = null, Guid? userTypeId = null, CancellationToken ct = default)
    {
        await loginAudit.LogAsync(new LoginAttemptEntry(userId, userTypeId ?? DefaultUserType, Provider, false, reason), ct);
        return Result.Fail<AuthResponse>(reason);
    }

    protected async Task<IResult<AuthResponse>> LogFailureAsync(IEnumerable<IError> errors, 
        Guid? userId = null, Guid? userTypeId = null, CancellationToken ct = default)
    {
        var errorList = errors.ToList();
        var reason = string.Join(", ", errorList.Select(e => e.Message));
        await loginAudit.LogAsync(new LoginAttemptEntry(userId, userTypeId ?? DefaultUserType, Provider, false, reason), ct);
        return Result.Fail<AuthResponse>(errorList);
    }
}
