namespace Tawtheef.Application.Features.Lookups.Queries;

public record BaseSearchQuery
{
    public Guid? Id { get; init; }
    public string? Search { get; init; }
    public string Language { get; init; } = "en";
}
