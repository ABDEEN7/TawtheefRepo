using Mapster;
using Tawtheef.Application.Common.Models.Pagination;

namespace Tawtheef.Application.Extensions;

public static class PaginatedResultExtensions
{
    public static PaginatedResult<TDestination> AdaptPaginated<TSource, TDestination>(
        this PaginatedResult<TSource>? source)
    {
        if (source == null) return null!;

        var items = source.Items.Adapt<List<TDestination>>();

        return new PaginatedResult<TDestination>(
            items,
            source.Metadata.TotalCount,
            source.Metadata.CurrentPage,
            source.Metadata.PageSize);
    }
}
