namespace Tawtheef.Domain.ValueObjects;

public sealed class GraphMailRequest
{
    public required string Subject { get; init; }
    public string? HtmlBody { get; init; }
    public string? TextBody { get; init; }

    public IReadOnlyCollection<string> To { get; init; } = Array.Empty<string>();
    public IReadOnlyCollection<string>? Cc { get; init; }
}
