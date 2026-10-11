using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Concurrency;
using Tawtheef.Infrastructure.Data;

namespace Tawtheef.Infrastructure.Services.Concurrency;

public sealed class SqlServerApplicationLockService(TawtheefDbContext dbContext) : IApplicationLockService
{
    private const int LockTimeoutMilliseconds = 15_000;
    private const int MaxResourceLength = 255;

    public Task AcquireExclusiveAsync(string resource, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(resource);

        if (resource.Length > MaxResourceLength)
        {
            throw new ArgumentOutOfRangeException(nameof(resource),
                $"Application lock resource cannot exceed {MaxResourceLength} characters.");
        }

        if (dbContext.Database.CurrentTransaction is null)
        {
            throw new InvalidOperationException(
                "Application locks must be acquired inside an active database transaction.");
        }

        return dbContext.Database.ExecuteSqlInterpolatedAsync($"""
                                                               DECLARE @result int;

                                                               EXEC @result = sys.sp_getapplock
                                                                   @Resource = {resource},
                                                                   @LockMode = N'Exclusive',
                                                                   @LockOwner = N'Transaction',
                                                                   @LockTimeout = {LockTimeoutMilliseconds};

                                                               IF @result < 0
                                                                   THROW 51000, 'Application lock unavailable', 1;
                                                               """, cancellationToken);
    }
}
