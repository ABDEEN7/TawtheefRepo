using Tawtheef.Application.Common.Models.Pagination;

namespace Tawtheef.Application.Features.Lookups.Queries;

public record BaseSearchQuery
{
    public Guid? Id { get; init; }
    public string? Search { get; init; }
    public PaginatedRequest? PaginatedRequest { get; init; } = null!;
    public string Language { get; init; } = "en";
}
