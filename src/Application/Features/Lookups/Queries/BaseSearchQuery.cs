namespace Tawtheef.Application.Features.Lookups.Queries;

public record BaseSearchQuery
{
    public Guid? Id { get; init; }
    public string? Search { get; init; }
    public int? PageIndex { get; init; }
    public int? PageSize { get; init; }
    public string Language { get; init; } = "en";
}
