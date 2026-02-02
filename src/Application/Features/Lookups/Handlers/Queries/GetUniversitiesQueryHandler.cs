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
        var isPaged = request.PaginatedRequest is not null;
        var languageToken = string.IsNullOrWhiteSpace(request.Language)
            ? "en"
            : request.Language.Trim().ToLowerInvariant();
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
        if (isPaged)
            cacheKeyPrefix = $"{cacheKeyPrefix}:page:{request.PaginatedRequest?.PageNumber}:{request.PaginatedRequest?.PageSize}";
        var cacheKey = await LookupCacheKeyBuilder.BuildAsync(query, cacheKeyPrefix, cancellationToken);

        var universities = await cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.SetSlidingExpiration(TimeSpan.FromMinutes(30));

            if (isPaged)
            {
                var ordered = languageToken == "ar"
                    ? query.OrderBy(university => university.DisplayOrder).ThenBy(university => university.NameAr)
                    : query.OrderBy(university => university.DisplayOrder).ThenBy(university => university.NameEn);
                if (request.PaginatedRequest != null)
                {
                    var entities = await ordered.ToPaginatedListAsync(request.PaginatedRequest,cancellationToken);
                    return mapper.Map<List<DropdownOptions>>(entities);
                }
            }

            var allEntities = await query
                .OrderBy(university => university.DisplayOrder)
                .ToListAsync(cancellationToken);
            return mapper.Map<List<DropdownOptions>>(allEntities);
        });

        if (request.Id.HasValue)
        {
            var byId = await unitOfWork.GetEntityRepository<University>().DbSet
                .AsNoTracking()
                .Where(u => u.IsActive)
                .Where(u => u.Id == request.Id.Value)
                .ToListAsync(cancellationToken);
            var mappedById = mapper.Map<List<DropdownOptions>>(byId);
            var merged = universities ?? new List<DropdownOptions>();
            foreach (var item in mappedById.Where(item => merged.All(existing => existing.Id != item.Id)))
            {
                merged.Add(item);
            }

            return Result.Ok(merged);
        }

        return Result.Ok(universities ?? new List<DropdownOptions>());
    }
}
