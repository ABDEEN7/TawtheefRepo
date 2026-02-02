using Cortex.Mediator.Queries;
using FluentResults;
using MapsterMapper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.EntityFrameworkCore;
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
        var isPaged = request.PageSize.HasValue || request.PageIndex.HasValue;
        var pageSize = request.PageSize ?? 10;
        var pageIndex = request.PageIndex ?? 0;
        var cacheKeyPrefix = $"{CacheKeyPrefix}:{typeof(TLookup).Name}:{languageToken}:{searchToken}";
        if (isPaged)
            cacheKeyPrefix = $"{cacheKeyPrefix}:page:{pageIndex}:{pageSize}";
        var cacheKey = await LookupCacheKeyBuilder.BuildAsync(dbSet, cacheKeyPrefix, cancellationToken);

        var data = await cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.SetSlidingExpiration(TimeSpan.FromMinutes(30));

            if (isPaged)
            {
                var ordered = languageToken == "ar"
                    ? dbSet.OrderBy(x => x.DisplayOrder).ThenBy(x => x.NameAr)
                    : dbSet.OrderBy(x => x.DisplayOrder).ThenBy(x => x.NameEn);
                var entities = await ordered
                    .Skip(pageIndex * pageSize)
                    .Take(pageSize)
                    .ToListAsync(cancellationToken);
                return mapper.Map<List<DropdownOptions>>(entities);
            }

            var allEntities = await dbSet.ToListAsync(cancellationToken);
            return mapper.Map<List<DropdownOptions>>(allEntities).OrderBy(e => e.Name).ToList();
        });

        if (request.Id.HasValue)
        {
            var byId = await unitOfWork.GetEntityRepository<TLookup>().DbSet
                .AsNoTracking()
                .Where(x => x.IsActive && x.Id == request.Id.Value)
                .ToListAsync(cancellationToken);
            var mappedById = mapper.Map<List<DropdownOptions>>(byId);
            var merged = data ?? new List<DropdownOptions>();
            foreach (var item in mappedById)
            {
                if (!merged.Any(existing => existing.Id == item.Id))
                {
                    merged.Add(item);
                }
            }

            return Result.Ok(merged);
        }

        return Result.Ok(data ?? new List<DropdownOptions>());
    }
}

public static class LookupCacheKeyBuilder
{
    public static async Task<string> BuildAsync<TEntity>(
        IQueryable<TEntity> query,
        string prefix,
        CancellationToken cancellationToken)
        where TEntity : BaseEntity
    {
        if (!await query.AnyAsync(cancellationToken))
        {
            return $"{prefix}:empty";
        }

        var count = await query.CountAsync(cancellationToken);
        var lastUpdated = await query.MaxAsync(
            entity => entity.UpdatedDate ?? entity.CreatedDate,
            cancellationToken);

        return $"{prefix}:{count}:{lastUpdated.UtcTicks}";
    }
}
