namespace Tawtheef.Domain.Configurations.Settings;

public sealed class EmailDispatcherSettings
{
    // parallel consumers
    public int Workers { get; init; } = 3;
    public int MaxConsecutiveFailuresBeforeBackoff { get; init; } = 5;
    // base backoff (with jitter)
    public int BackoffMs { get; init; } = 500;
    // poison protection (transport may also retry)
    public int MaxAttemptsPerEnvelope { get; init; } = 3;
    // finish queue on shutdown
    public bool DrainOnStop { get; init; } = true;
    public TimeSpan MaxDrainDuration { get; init; } = TimeSpan.FromSeconds(30);
}