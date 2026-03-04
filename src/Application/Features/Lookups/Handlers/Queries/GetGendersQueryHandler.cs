using Cortex.Mediator.Queries;
using FluentResults;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Models;
using Tawtheef.Application.Features.Lookups.Queries;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Application.Features.Lookups.Handlers.Queries;

public sealed class GetGendersQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IMemoryCache cache) 
    : IQueryHandler<GetGendersQuery, IResult<List<DropdownOptions>>>
{
    private const string CacheKey = "lookups:Gender:all:en";

    public async Task<IResult<List<DropdownOptions>>> Handle(GetGendersQuery request, CancellationToken cancellationToken)
    {
        var data = await cache.GetOrCreateAsync(CacheKey, async entry =>
        {
            entry.SetSlidingExpiration(TimeSpan.FromHours(1));

            var dbSet = unitOfWork.GetEntityRepository<Gender>().DbSet;

            var entities = await dbSet
                .AsNoTracking()
                .Skip(1)
                .OrderBy(x => x.DisplayOrder)
                .ToListAsync(cancellationToken);

            return mapper.Map<List<DropdownOptions>>(entities);
        });

        return Result.Ok(data ?? new List<DropdownOptions>());
    }
}
