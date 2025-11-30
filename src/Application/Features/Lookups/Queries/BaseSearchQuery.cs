namespace Tawtheef.Application.Features.Lookups.Queries;

public record BaseSearchQuery
{
    public string? Search { get; set; }
    public string? Language { get; set; }
}
