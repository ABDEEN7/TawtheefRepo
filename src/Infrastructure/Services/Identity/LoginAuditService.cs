using Microsoft.AspNetCore.Http;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Domain.Entities.Auth;
using Tawtheef.Infrastructure.Extensions;

namespace Tawtheef.Infrastructure.Services.Identity;

public sealed class LoginAuditService(
    IUnitOfWork uow,
    IHttpContextAccessor httpContextAccessor,
    TimeProvider time) : ILoginAuditService
{
    public async Task LogAsync(LoginAttemptEntry entry, CancellationToken ct)
    {
        var ip = entry.IpAddress ?? httpContextAccessor.HttpContext?.GetClientIpAddress();
        var attempt = new LoginAttempt
        {
            Id = Guid.NewGuid(),
            UserId = entry.UserId,
            UserTypeId = entry.UserTypeId,
            Source = entry.Source,
            Succeeded = entry.Succeeded,
            FailureReason = entry.FailureReason,
            SessionId = entry.SessionId,
            AttemptedAtUtc = entry.AttemptedAt?.UtcDateTime ?? time.GetUtcNow().UtcDateTime,
            IpAddress = ip
        };

        await uow.GetEntityRepository<LoginAttempt>().AddAsync(attempt);
        await uow.SaveChangesAsync(ct);
    }
}
