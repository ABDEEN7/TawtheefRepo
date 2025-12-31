namespace Tawtheef.Application.Features.Lookups.Queries;

public record BaseSearchQuery
{
    public string? Search { get; init; }
    public string Language { get; init; } = "en";
}
