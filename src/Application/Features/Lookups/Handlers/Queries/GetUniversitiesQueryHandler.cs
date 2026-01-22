using Cortex.Mediator.Queries;
using FluentResults;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Models;
using Tawtheef.Application.Features.Lookups.Queries;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Tawtheef.Application.Features.Lookups.Handlers.Queries;

public sealed class GetUniversitiesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    : IQueryHandler<GetUniversitiesQuery, IResult<List<DropdownOptions>>>
{
    public async Task<IResult<List<DropdownOptions>>> Handle(GetUniversitiesQuery request, CancellationToken cancellationToken)
    {
        var baseQuery = unitOfWork.GetEntityRepository<University>().DbSet
            .AsNoTracking()
            .Where(u => u.IsActive)
            .Where(u => u.City!.CountryId == request.CountryId);

        List<University> byId = [];
        if (request.Id.HasValue)
        {
            byId = await baseQuery
                .Where(u => u.Id == request.Id.Value)
                .ToListAsync(cancellationToken);
        }

        List<University> bySearch = [];
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var s = request.Search.Trim();

            bySearch = await baseQuery
                .Where(university =>
                    EF.Functions.Like(university.NameAr, $"%{s}%") ||
                    EF.Functions.Like(university.NameEn, $"%{s}%") ||
                    EF.Functions.Like(university.DescriptionAr ?? "", $"%{s}%") ||
                    EF.Functions.Like(university.DescriptionEn ?? "", $"%{s}%"))
                .OrderBy(university => university.DisplayOrder)
                .Take(10)
                .ToListAsync(cancellationToken);
        }

        var merged = byId
            .Concat(bySearch)
            .GroupBy(u => u.Id)
            .Select(g => g.First())
            .OrderBy(u => u.DisplayOrder)
            .ToList();

        return Result.Ok(mapper.Map<List<DropdownOptions>>(merged));
    }
}

