using Cortex.Mediator.Queries;
using FluentResults;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Models;
using Tawtheef.Application.Extensions;
using Tawtheef.Application.Features.Lookups.Queries;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Tawtheef.Application.Features.Lookups.Handlers.Queries;

public sealed class GetUniversitiesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IMemoryCache cache)
    : IQueryHandler<GetUniversitiesQuery, IResult<List<DropdownOptions>>>
{
    private const string CacheKeyPrefix = "lookups:universities";

    public async Task<IResult<List<DropdownOptions>>> Handle(GetUniversitiesQuery request, CancellationToken cancellationToken)
    {
        var normalizedSearch = request.Search?.Trim();
        var query = unitOfWork.GetEntityRepository<University>().DbSet
            .AsNoTracking()
            .Where(s => s.IsActive)
            .Where(u => u.City!.CountryId == request.CountryId)
            .WhereIf(!string.IsNullOrEmpty(normalizedSearch),
                university =>
                    EF.Functions.Like(university.NameAr, $"%{normalizedSearch}%") ||
                    EF.Functions.Like(university.NameEn, $"%{normalizedSearch}%") ||
                    EF.Functions.Like(university.DescriptionAr ?? "", $"%{normalizedSearch}%") ||
                    EF.Functions.Like(university.DescriptionEn ?? "", $"%{normalizedSearch}%"));

        var searchToken = string.IsNullOrEmpty(normalizedSearch)
            ? "all"
            : normalizedSearch.ToLowerInvariant();
        var cacheKeyPrefix = $"{CacheKeyPrefix}:{request.CountryId}:{searchToken}";
        var cacheKey = await LookupCacheKeyBuilder.BuildAsync(query, cacheKeyPrefix, cancellationToken);

        var universities = await cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.SetSlidingExpiration(TimeSpan.FromMinutes(30));

            var entities = await query
                .OrderBy(university => university.DisplayOrder)
                .ToListAsync(cancellationToken);
            return mapper.Map<List<DropdownOptions>>(entities);
        });

        return Result.Ok(universities ?? new List<DropdownOptions>());
    }
}
