namespace Tawtheef.Domain.ValueObjects;

public sealed class GraphMailRequest
{
    public required string Subject { get; init; }
    public string? HtmlBody { get; init; }
    public string? TextBody { get; init; }

    public IReadOnlyCollection<string> To { get; init; } = Array.Empty<string>();
    public IReadOnlyCollection<string>? Cc { get; init; }

    public List<GraphMailAttachment>? Attachments { get; set; }
}

public sealed class GraphMailAttachment
{
    public required string Name { get; init; }
    public required string ContentType { get; init; }
    public required byte[] ContentBytes { get; init; }
    public string? ContentId { get; init; }
    public bool IsInline { get; init; }
}
