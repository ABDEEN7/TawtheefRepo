using MediatR;
using FluentResults;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Models;
using Tawtheef.Application.Extensions;
using Tawtheef.Application.Features.Lookups.Queries;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Tawtheef.Application.Features.Lookups.Handlers.Queries;

public sealed class GetCountriesQueryHandler(IUnitOfWork unitOfWork, IMemoryCache cache)
    : IRequestHandler<GetCountriesQuery, IResult<List<DropdownOptions>>>
{
    private const string CacheKeyPrefix = "lookups:countries";

    public async Task<IResult<List<DropdownOptions>>> Handle(GetCountriesQuery request,
        CancellationToken cancellationToken)
    {
        //get language from the header request
        var language = request.Language;

        var query = unitOfWork.GetEntityRepository<Country>()
            .DbSet
            .AsNoTracking()
            .Where(s => s.IsActive);

        var cacheKeyPrefix = $"{CacheKeyPrefix}:{language}";
        var cacheKey = await LookupCacheKeyBuilder.BuildAsync(query, cacheKeyPrefix, cancellationToken);

        var countries = await cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.SetSlidingExpiration(TimeSpan.FromMinutes(30));

            return await query
                .Select(c => new DropdownOptions
                {
                    Id = c.Id,
                    Name = c.GetLocalizedName(language)!,
                    Description = c.GetLocalizedDescription(language)!,
                    BackendName = c.BackendName,
                    AdditionalData = new
                    {
                        c.NameAr,
                        c.NameEn,
                        c.Code,
                        c.ISOCode,
                        c.CodeAlpha
                    }
                })
                .ToListAsync(cancellationToken);
        });

        return Result.Ok(countries?.ToList() ?? new List<DropdownOptions>());
    }
}

