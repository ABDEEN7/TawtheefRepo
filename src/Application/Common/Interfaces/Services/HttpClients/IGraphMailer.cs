using Tawtheef.Domain.ValueObjects;

namespace Tawtheef.Application.Common.Interfaces.Services.HttpClients;

public interface IGraphMailer
{
    Task SendAsync(GraphMailRequest request, CancellationToken cancellationToken = default);
}
