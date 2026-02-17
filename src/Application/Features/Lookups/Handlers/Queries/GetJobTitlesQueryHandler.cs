using Cortex.Mediator.Queries;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Models;
using Tawtheef.Application.Extensions;
using Tawtheef.Application.Features.Lookups.Queries;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Tawtheef.Application.Features.Lookups.Handlers.Queries;

public sealed class GetJobTitlesQueryHandler(IUnitOfWork unitOfWork)
    : IQueryHandler<GetJobTitlesQuery, IResult<List<DropdownOptions>>>
{
    public async Task<IResult<List<DropdownOptions>>> Handle(GetJobTitlesQuery request, CancellationToken cancellationToken)
    {
        var dbSet = unitOfWork.GetEntityRepository<JobTitle>().DbSet;
        var baseQuery = dbSet
            .AsNoTracking()
            .Where(s => s.IsActive);
        var normalizedSearch = request.Search?.Trim();
        var isPaged = request.PaginatedRequest is not null;


        List<JobTitle> byId = [];
        if (request.Id.HasValue)
        {
            byId = await baseQuery
                .Where(m => m.Id == request.Id.Value)
                .ToListAsync(cancellationToken);
        }

        var bySearch = new List<JobTitle>();
        if (!string.IsNullOrWhiteSpace(normalizedSearch) || isPaged)
        {
            var searchQuery = baseQuery;
            if (!string.IsNullOrWhiteSpace(normalizedSearch))
            {
                searchQuery = searchQuery.Where(m =>
                    EF.Functions.Like(m.JobNameAr, $"%{normalizedSearch}%") ||
                    EF.Functions.Like(m.JobNameEn, $"%{normalizedSearch}%"));
            }

            if (request.PaginatedRequest != null)
                bySearch = await searchQuery.ToPaginatedResultAsync(request.PaginatedRequest, cancellationToken);
        }

        var merged = byId
            .Concat(bySearch)
            .GroupBy(m => m.Id)
            .Select(g => g.First())
            .OrderBy(m => m.JobNameAr)
            .ToList();

        var result = merged.Select(x => new DropdownOptions
        {
            Id = x.Id,
            Name = request.Language == "ar" ? x.JobNameAr : x.JobNameEn,
            AdditionalData = new { NameAr = x.JobNameAr, NameEn = x.JobNameEn, x.JobNumber }
        }).ToList();
        
        return Result.Ok(result);
    }
}
