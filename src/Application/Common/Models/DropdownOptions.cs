namespace Tawtheef.Application.Common.Models;

public record DropdownOptions
{
    public Guid Id { get; init; }
    public string BackendName { get; init; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}
