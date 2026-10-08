namespace Tawtheef.Application.Common.Interfaces.Concurrency;

public interface IApplicationLockService
{
    Task AcquireExclusiveAsync(string resource, CancellationToken cancellationToken);
}
