using System.Linq.Expressions;
using System.Reflection;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Models.Pagination;

namespace Tawtheef.Application.Extensions;

public static class QueryableExtensions
{
    extension<TSource>(IQueryable<TSource> source) where TSource : class
    {
        public async Task<List<TSource>> ToPaginatedResultAsync(PaginatedRequest paginatedRequest,
            CancellationToken cancellationToken = default)
        {
            var items = await source
                .SortBy(paginatedRequest.SortBy, paginatedRequest.SortDirection)
                .Skip((paginatedRequest.PageNumber - 1) * paginatedRequest.PageSize)
                .Take(paginatedRequest.PageSize)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return items;
        }
        public async Task<PaginatedResult<TSource>> ToPaginatedListAsync(PaginatedRequest paginatedRequest,
            CancellationToken cancellationToken = default)
        {
            var count = await source.CountAsync(cancellationToken);
            var items = await source
                .SortBy(paginatedRequest.SortBy, paginatedRequest.SortDirection)
                .Skip((paginatedRequest.PageNumber - 1) * paginatedRequest.PageSize)
                .Take(paginatedRequest.PageSize)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return new PaginatedResult<TSource>(items, count, paginatedRequest.PageNumber, paginatedRequest.PageSize);
        }

        public async Task<PaginatedResult<TDestination>> ToPaginatedListAsync<TDestination>(IMapper mapper,
            PaginatedRequest paginatedRequest,
            CancellationToken cancellationToken = default)
        {
            var count = await source.CountAsync(cancellationToken);
            var items = await source
                .SortBy(paginatedRequest.SortBy, paginatedRequest.SortDirection)
                .Skip((paginatedRequest.PageNumber - 1) * paginatedRequest.PageSize)
                .Take(paginatedRequest.PageSize)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return new PaginatedResult<TDestination>(mapper.Map<List<TDestination>>(items), count,
                paginatedRequest.PageNumber, paginatedRequest.PageSize);
        }
    }

    extension<T>(IQueryable<T> source)
    {
        public IQueryable<T> WhereIf(bool condition,
            Expression<Func<T, bool>> predicate)
        {
            return condition ? source.Where(predicate) : source;
        }

        private IQueryable<T> SortBy(string? sortBy, string? sortDirection)
        {
            if (string.IsNullOrWhiteSpace(sortBy))
                return source;

            var isDescending = string.Equals(sortDirection, "desc", StringComparison.OrdinalIgnoreCase);
            var parameter = Expression.Parameter(typeof(T), "x");

            // Build x => x.Prop OR x => x.Nested.Prop
            var body = BuildPropertyAccess(parameter, typeof(T), sortBy);
            if (body is null) // property isn't found -> no sort
                return source;

            var keyType = body.Type;
            var lambda = Expression.Lambda(
                typeof(Func<,>).MakeGenericType(typeof(T), keyType), body, parameter);

            var method = typeof(Queryable).GetMethods()
                .First(m => m.Name == (isDescending ? "OrderByDescending" : "OrderBy")
                            && m.IsGenericMethodDefinition
                            && m.GetGenericArguments().Length == 2
                            && m.GetParameters().Length == 2);

            var ordered = method.MakeGenericMethod(typeof(T), keyType)
                .Invoke(null, [source, lambda])!;

            return (IQueryable<T>)ordered;
        }
    }

    private static Expression? BuildPropertyAccess(ParameterExpression param, Type type, string path)
    {
        Expression current = param;
        foreach (var part in path.Split('.', StringSplitOptions.RemoveEmptyEntries))
        {
            var prop = type.GetProperty(part,
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (prop is null) return null;
            current = Expression.Property(current, prop);
            type = prop.PropertyType;
        }

        return current;
    }

    public static async Task<TAccumulate> AggregateAsync<TSource, TAccumulate>(
        this IQueryable<TSource> source,
        TAccumulate seed,
        Func<TAccumulate, TSource, TAccumulate> func,
        CancellationToken cancellationToken = default)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (func == null) throw new ArgumentNullException(nameof(func));

        // For IQueryable, we might need to handle this differently for async operations
        // This is a simplified implementation that materializes the query first

        var items = await source.ToListAsync(cancellationToken).ConfigureAwait(false);

        var result = seed;
        foreach (var item in items)
        {
            result = func(result, item);
        }

        return result;
    }
}
