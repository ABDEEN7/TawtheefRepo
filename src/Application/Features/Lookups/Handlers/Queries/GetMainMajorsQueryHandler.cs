using MediatR;
using FluentResults;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Models;
using Tawtheef.Application.Extensions;
using Tawtheef.Application.Features.Lookups.Queries;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Tawtheef.Application.Features.Lookups.Handlers.Queries;

public sealed class GetMainMajorsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    : IRequestHandler<GetMainMajorsQuery, IResult<List<DropdownOptions>>>
{
    public async Task<IResult<List<DropdownOptions>>> Handle(GetMainMajorsQuery request, CancellationToken cancellationToken)
    {
        var dbSet = unitOfWork.GetEntityRepository<Major>().DbSet;
        var baseQuery = dbSet
            .AsNoTracking()
            .Where(m => m.IsActive)
            .Where(m=> m.ParentId == null)
            .WhereIf(!request.IncludeOrphanMajors, m => m.SubMajors!.Count > 0);
        var normalizedSearch = request.Search?.Trim();
        List<Major> byId = [];
        if (request.Id.HasValue)
        {
            byId = await baseQuery
                .Where(m => m.Id == request.Id.Value)
                .ToListAsync(cancellationToken);
        }

        var searchQuery = baseQuery;
        if (!string.IsNullOrWhiteSpace(normalizedSearch))
        {
            searchQuery = searchQuery.Where(m =>
                EF.Functions.Like(m.NameAr, $"%{normalizedSearch}%") ||
                EF.Functions.Like(m.NameEn, $"%{normalizedSearch}%") ||
                EF.Functions.Like(m.DescriptionAr ?? "", $"%{normalizedSearch}%") ||
                EF.Functions.Like(m.DescriptionEn ?? "", $"%{normalizedSearch}%"));
        }

        List<Major> bySearch;
        if (request.PaginatedRequest is not null)
        {
            bySearch = await searchQuery.ToPaginatedResultAsync(request.PaginatedRequest, cancellationToken);
        }
        else if (!string.IsNullOrWhiteSpace(normalizedSearch) || !request.Id.HasValue)
        {
            bySearch = await searchQuery.ToListAsync(cancellationToken);
        }
        else
        {
            bySearch = [];
        }

        var merged = byId
            .Concat(bySearch)
            .GroupBy(m => m.Id)
            .Select(g => g.First())
            .OrderBy(m => m.DisplayOrder)
            .ToList();

        return Result.Ok(mapper.Map<List<DropdownOptions>>(merged));
    }
}

