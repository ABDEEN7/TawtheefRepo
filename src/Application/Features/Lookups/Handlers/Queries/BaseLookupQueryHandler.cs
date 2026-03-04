using Cortex.Mediator.Queries;
using FluentResults;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Models;
using Tawtheef.Application.Extensions;
using Tawtheef.Application.Features.Lookups.Queries;
using Tawtheef.Domain.Common;

namespace Tawtheef.Application.Features.Lookups.Handlers.Queries;

public abstract class BaseLookupQueryHandler<TLookup, TRequest>(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IMemoryCache cache)
    : IQueryHandler<TRequest, IResult<List<DropdownOptions>>>
    where TLookup : LookupBase
    where TRequest : BaseSearchQuery, IQuery<IResult<List<DropdownOptions>>>
{
    private const string CacheKeyPrefix = "lookups";

    public async Task<IResult<List<DropdownOptions>>> Handle(TRequest request, CancellationToken cancellationToken)
    {
        var normalizedSearch = request.Search?.Trim();
        var dbSet = unitOfWork.GetEntityRepository<TLookup>().DbSet
            .AsNoTracking()
            .Where(x => x.IsActive)
            .WhereIf(!string.IsNullOrEmpty(normalizedSearch),
                s =>
                    EF.Functions.Like(s.NameAr, $"%{normalizedSearch}%") ||
                    EF.Functions.Like(s.NameEn, $"%{normalizedSearch}%") ||
                    EF.Functions.Like(s.DescriptionAr ?? "", $"%{normalizedSearch}%") ||
                    EF.Functions.Like(s.DescriptionEn ?? "", $"%{normalizedSearch}%"));

        var searchToken = string.IsNullOrEmpty(normalizedSearch)
            ? "all"
            : normalizedSearch.ToLowerInvariant();
        var languageToken = string.IsNullOrWhiteSpace(request.Language)
            ? "en"
            : request.Language.Trim().ToLowerInvariant();
        
        var isPaged = request.PaginatedRequest is not null;
        var cacheKeyPrefix = $"{CacheKeyPrefix}:{typeof(TLookup).Name}:{languageToken}:{searchToken}";
        if (isPaged)
            cacheKeyPrefix = $"{cacheKeyPrefix}:page:{request.PaginatedRequest?.PageNumber}:{request.PaginatedRequest?.PageSize}";
        var cacheKey = cacheKeyPrefix;

        var data = await cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.SetSlidingExpiration(TimeSpan.FromHours(1)); // Increased TTL for lookups

            if (isPaged && request.PaginatedRequest != null)
            {
                    var entities = await dbSet.ToPaginatedListAsync(request.PaginatedRequest, cancellationToken);
                    return mapper.Map<List<DropdownOptions>>(entities.Items);
            }

            var allEntities = await dbSet.ToListAsync(cancellationToken);
            return mapper.Map<List<DropdownOptions>>(allEntities).OrderBy(e => e.Name).ToList();
        });

        if (request is { Id: not null })
        {
            var byId = await unitOfWork.GetEntityRepository<TLookup>().DbSet
                .AsNoTracking()
                .Where(x => x.IsActive && x.Id == request.Id.Value)
                .ToListAsync(cancellationToken);
            var mappedById = mapper.Map<List<DropdownOptions>>(byId);
            var merged = data ?? new List<DropdownOptions>();
            foreach (var item in mappedById.Where(item => merged.All(existing => existing.Id != item.Id)))
            {
                merged.Add(item);
            }

            return Result.Ok(merged);
        }

        return Result.Ok(data ?? new List<DropdownOptions>());
    }
}

public static class LookupCacheKeyBuilder
{
    public static Task<string> BuildAsync<TEntity>(
        IQueryable<TEntity> query,
        string prefix,
        CancellationToken cancellationToken)
        where TEntity : BaseEntity
    {
        // Optimization: Avoid redundant database queries (Any, Count, Max) just for cache keys.
        // For lookups, a TTL-based cache on the prefix is much more performant.
        return Task.FromResult(prefix);
    }
}

