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

public sealed class GetSubMajorsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) 
    : IRequestHandler<GetSubMajorsQuery, IResult<List<DropdownOptions>>>
{
    public async Task<IResult<List<DropdownOptions>>> Handle(GetSubMajorsQuery request, CancellationToken cancellationToken)
    {
        var dbSet = unitOfWork.GetEntityRepository<Major>().DbSet;
        var baseQuery = dbSet
            .AsNoTracking()
            .Where(s => s.IsActive && !s.IsDeleted)
            .Where(x => x.ParentId == request.ParentId);
        var normalizedSearch = request.Search?.Trim();
        var isPaged = request.PaginatedRequest is not  null;
        
        List<Major> byId = [];
        if (request.Id.HasValue)
        {
            byId = await baseQuery
                .Where(m => m.Id == request.Id.Value)
                .ToListAsync(cancellationToken);
        }
        
        var bySearch = new List<Major>();
        if (!string.IsNullOrWhiteSpace(normalizedSearch) || isPaged)
        {
            var searchQuery = baseQuery;
            if (!string.IsNullOrWhiteSpace(normalizedSearch))
            {
                searchQuery = searchQuery.Where(m =>
                    EF.Functions.Like(m.NameAr, $"%{normalizedSearch}%") ||
                    EF.Functions.Like(m.NameEn, $"%{normalizedSearch}%") ||
                    EF.Functions.Like(m.DescriptionAr ?? "", $"%{normalizedSearch}%") ||
                    EF.Functions.Like(m.DescriptionEn ?? "", $"%{normalizedSearch}%"));
            }

            if (request.PaginatedRequest != null)
                bySearch = await searchQuery.ToPaginatedResultAsync(request.PaginatedRequest, cancellationToken);
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

