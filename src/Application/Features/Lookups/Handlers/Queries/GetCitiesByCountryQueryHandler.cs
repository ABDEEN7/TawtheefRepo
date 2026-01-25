using Cortex.Mediator.Queries;
using FluentResults;
using MapsterMapper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Models;
using Tawtheef.Application.Extensions;
using Tawtheef.Application.Features.Lookups.Queries;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Tawtheef.Application.Features.Lookups.Handlers.Queries;

public sealed class GetCitiesByCountryQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IMemoryCache cache)
    : IQueryHandler<GetCitiesByCountryQuery, IResult<List<DropdownOptions>>>
{
    private const string CacheKeyPrefix = "lookups:cities";

    public async Task<IResult<List<DropdownOptions>>> Handle(GetCitiesByCountryQuery request, CancellationToken cancellationToken)
    {
        var normalizedSearch = request.Search?.Trim();
        var query = unitOfWork.GetEntityRepository<City>().DbSet
            .AsNoTracking()
            .Where(s => s.IsActive)
            .Where(c => c.CountryId == request.CountryId)
            .WhereIf(!string.IsNullOrEmpty(normalizedSearch),
                c => EF.Functions.Like(c.NameAr, $"%{normalizedSearch}%") ||
                     EF.Functions.Like(c.NameEn, $"%{normalizedSearch}%") ||
                     EF.Functions.Like(c.DescriptionAr ?? string.Empty, $"%{normalizedSearch}%") ||
                     EF.Functions.Like(c.DescriptionEn ?? string.Empty, $"%{normalizedSearch}%"));

        var searchToken = string.IsNullOrEmpty(normalizedSearch)
            ? "all"
            : normalizedSearch.ToLowerInvariant();
        var cacheKeyPrefix = $"{CacheKeyPrefix}:{request.CountryId}:{searchToken}";
        var cacheKey = await LookupCacheKeyBuilder.BuildAsync(query, cacheKeyPrefix, cancellationToken);

        var data = await cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.SetSlidingExpiration(TimeSpan.FromMinutes(30));

            var entities = await query.ToListAsync(cancellationToken);
            return mapper.Map<List<DropdownOptions>>(entities).OrderBy(e => e.Name).ToList();
        });

        return Result.Ok(data ?? new List<DropdownOptions>());
    }
}
