using Tawtheef.Application.Common.Interfaces.Services.Security;

namespace Tawtheef.Infrastructure.Services.Security;

public sealed class IdentityFieldProtectionContext : IIdentityFieldProtectionContext
{
    private readonly AsyncLocal<int> _trustedDepth = new();

    public bool AllowVerifiedIdentityWrite => _trustedDepth.Value > 0;

    public IDisposable BeginTrustedIdentityWriteScope()
    {
        _trustedDepth.Value++;
        return new TrustedScope(this);
    }

    private sealed class TrustedScope(IdentityFieldProtectionContext owner) : IDisposable
    {
        private bool _disposed;

        public void Dispose()
        {
            if (_disposed)
                return;

            owner._trustedDepth.Value = Math.Max(0, owner._trustedDepth.Value - 1);
            _disposed = true;
        }
    }
}
