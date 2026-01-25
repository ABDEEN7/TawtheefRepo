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

public sealed class GetOfficesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IMemoryCache cache)
    : IQueryHandler<GetOfficesQuery, IResult<List<DropdownOptions>>>
{
    private const string CacheKeyPrefix = "lookups:offices";

    public async Task<IResult<List<DropdownOptions>>> Handle(GetOfficesQuery request, CancellationToken cancellationToken)
    {
        var normalizedSearch = request.Search?.Trim();
        var query = unitOfWork.GetEntityRepository<Office>()
            .DbSet
            .AsNoTracking()
            .Where(s => s.IsActive)
            .WhereIf(request.CountryId.HasValue, o => o.CountryId == request.CountryId!.Value)
            .WhereIf(!string.IsNullOrWhiteSpace(normalizedSearch), o =>
                EF.Functions.Like(o.NameAr, $"%{normalizedSearch}%") ||
                EF.Functions.Like(o.NameEn, $"%{normalizedSearch}%"));

        var searchToken = string.IsNullOrEmpty(normalizedSearch)
            ? "all"
            : normalizedSearch.ToLowerInvariant();
        var countryToken = request.CountryId?.ToString() ?? "all";
        var cacheKeyPrefix = $"{CacheKeyPrefix}:{countryToken}:{searchToken}";
        var cacheKey = await LookupCacheKeyBuilder.BuildAsync(query, cacheKeyPrefix, cancellationToken);

        var offices = await cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.SetSlidingExpiration(TimeSpan.FromMinutes(30));

            var entities = await query
                .OrderBy(o => o.DisplayOrder)
                .ToListAsync(cancellationToken);
            return mapper.Map<List<DropdownOptions>>(entities);
        });

        return Result.Ok(offices ?? new List<DropdownOptions>());
    }
}
