using System.Reflection;
using MapsterMapper;
using Tawtheef.Application.Common.Models.Pagination;

namespace Tawtheef.Application.Extensions;

public static class EnumerableExtensions
{
    /* ----------------------------------------------------
     * Pagination
     * -------------------------------------------------- */

    extension<TSource>(IList<TSource> source)
    {
        public List<TSource> ToPaginatedResult(PaginatedRequest paginatedRequest)
        {
            return source
                .SortBy(paginatedRequest.SortBy, paginatedRequest.SortDirection)
                .Skip((paginatedRequest.PageNumber - 1) * paginatedRequest.PageSize)
                .Take(paginatedRequest.PageSize)
                .ToList();
        }

        public PaginatedResult<TSource> ToPaginatedList(PaginatedRequest paginatedRequest)
        {
            var count = source.Count();

            var items = source
                .SortBy(paginatedRequest.SortBy, paginatedRequest.SortDirection)
                .Skip((paginatedRequest.PageNumber - 1) * paginatedRequest.PageSize)
                .Take(paginatedRequest.PageSize)
                .ToList();

            return new PaginatedResult<TSource>(
                items,
                count,
                paginatedRequest.PageNumber,
                paginatedRequest.PageSize);
        }

        public PaginatedResult<TDestination> ToPaginatedList<TDestination>(IMapper mapper,
            PaginatedRequest paginatedRequest)
        {
            var count = source.Count();

            var items = source
                .SortBy(paginatedRequest.SortBy, paginatedRequest.SortDirection)
                .Skip((paginatedRequest.PageNumber - 1) * paginatedRequest.PageSize)
                .Take(paginatedRequest.PageSize)
                .ToList();

            return new PaginatedResult<TDestination>(
                mapper.Map<List<TDestination>>(items),
                count,
                paginatedRequest.PageNumber,
                paginatedRequest.PageSize);
        }

        public IEnumerable<TSource> WhereIf(bool condition,
            Func<TSource, bool> predicate)
        {
            return condition ? source.Where(predicate) : source;
        }

        private IEnumerable<TSource> SortBy(string? sortBy,
            string? sortDirection)
        {
            if (string.IsNullOrWhiteSpace(sortBy))
                return source;

            var prop = ResolveProperty(typeof(TSource), sortBy);
            if (prop is null)
                return source;

            var isDesc = string.Equals(sortDirection, "desc", StringComparison.OrdinalIgnoreCase);

            return isDesc
                ? source.OrderByDescending(x => prop.GetValue(x, null))
                : source.OrderBy(x => prop.GetValue(x, null));
        }
    }

    /* ----------------------------------------------------
     * Conditional Where
     * -------------------------------------------------- */

    /* ----------------------------------------------------
     * Sorting (Reflection-based, supports nested props)
     * -------------------------------------------------- */

    private static PropertyInfo? ResolveProperty(Type type, string path)
    {
        foreach (var part in path.Split('.', StringSplitOptions.RemoveEmptyEntries))
        {
            var prop = type.GetProperty(
                part,
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

            if (prop is null)
                return null;

            type = prop.PropertyType;
        }

        return type.GetProperty(path.Split('.').Last(),
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
    }

    /* ----------------------------------------------------
     * Aggregate
     * -------------------------------------------------- */

    public static TAccumulate AggregateSafe<TSource, TAccumulate>(
        this IEnumerable<TSource> source,
        TAccumulate seed,
        Func<TAccumulate, TSource, TAccumulate> func)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (func == null) throw new ArgumentNullException(nameof(func));

        var result = seed;
        foreach (var item in source)
        {
            result = func(result, item);
        }

        return result;
    }
}
