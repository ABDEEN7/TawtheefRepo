using Cortex.Mediator.Queries;
using FluentResults;
using MapsterMapper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Models;
using Tawtheef.Application.Features.Lookups.Queries;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Tawtheef.Application.Features.Lookups.Handlers.Queries;

public sealed class SearchSkillsHandler(IUnitOfWork uow, IMapper mapper, IMemoryCache cache)
    : IQueryHandler<SearchSkillsQuery, IResult<List<DropdownOptions>>>
{
    private const string CacheKeyPrefix = "lookups:skills:search";

    public async Task<IResult<List<DropdownOptions>>> Handle(
        SearchSkillsQuery request,
        CancellationToken cancellationToken)
    {
        var term = request.Search?.Trim().ToLower();
        if (!string.IsNullOrWhiteSpace(term) && term.Length < 3)
            return Result.Ok(new List<DropdownOptions>());

        var pageIndex = request.PaginatedRequest?.PageNumber ?? 0;
        var pageSize = request.PaginatedRequest?.PageSize ?? 10;

        var query = uow.GetEntityRepository<Skill>().DbSet
            .AsNoTracking()
            .Where(s => s.IsActive)
            .Where(s =>
                string.IsNullOrWhiteSpace(term) ||
                EF.Functions.Like(s.NameAr, $"%{term}%") ||
                EF.Functions.Like(s.NameEn, $"%{term}%") ||
                EF.Functions.Like(s.DescriptionAr ?? "", $"%{term}%") ||
                EF.Functions.Like(s.DescriptionEn ?? "", $"%{term}%"));

        var languageToken = string.IsNullOrWhiteSpace(request.Language)
            ? "en"
            : request.Language.Trim().ToLowerInvariant();
        var cacheKeyPrefix = $"{CacheKeyPrefix}:{languageToken}:{term ?? "all"}:{pageIndex}:{pageSize}";
        var cacheKey = await LookupCacheKeyBuilder.BuildAsync(query, cacheKeyPrefix, cancellationToken);

        var matches = await cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.SetSlidingExpiration(TimeSpan.FromMinutes(15));

            var entities = await query
                .OrderBy(s => s.DisplayOrder)
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
            return mapper.Map<List<DropdownOptions>>(entities);
        });

        return Result.Ok(matches ?? new List<DropdownOptions>());
    }
}
