using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities;

public class EmailTemplate : EventEntity
{
    public required string TemplateKey { get; set; }
    public required string Subject { get; set; }
    public required string BodyTemplate { get; set; }
    public bool IsActive { get; set; } = true;
}