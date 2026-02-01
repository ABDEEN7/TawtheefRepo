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
        var cacheKeyPrefix = $"{CacheKeyPrefix}:{typeof(TLookup).Name}:{languageToken}:{searchToken}";
        var cacheKey = await LookupCacheKeyBuilder.BuildAsync(dbSet, cacheKeyPrefix, cancellationToken);

        var data = await cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.SetSlidingExpiration(TimeSpan.FromMinutes(30));

            var entities = await dbSet.ToListAsync(cancellationToken);
            return mapper.Map<List<DropdownOptions>>(entities).OrderBy(e => e.Name).ToList();
        });

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
