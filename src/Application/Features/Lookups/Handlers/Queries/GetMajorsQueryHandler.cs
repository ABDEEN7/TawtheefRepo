using Cortex.Mediator.Queries;
using FluentResults;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Models;
using Tawtheef.Application.Features.Lookups.Queries;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Tawtheef.Application.Features.Lookups.Handlers.Queries;

public sealed class GetMajorsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    : IQueryHandler<GetMajorsQuery, IResult<List<DropdownOptions>>>
{
    public async Task<IResult<List<DropdownOptions>>> Handle(GetMajorsQuery request, CancellationToken cancellationToken)
    {
        var dbSet = unitOfWork.GetEntityRepository<Major>().DbSet;
        var baseQuery = dbSet
            .AsNoTracking()
            .Where(s => s.IsActive)
            .Where(m => m.SubMajors!.Count > 0);
        List<Major> byId = [];
        if (request.Id.HasValue)
        {
            byId = await baseQuery
                .Where(m => m.Id == request.Id.Value)
                .ToListAsync(cancellationToken);
        }

        List<Major> bySearch = [];
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            bySearch = await baseQuery
                .Where(m =>
                    EF.Functions.Like(m.NameAr, $"%{request.Search}%") ||
                    EF.Functions.Like(m.NameEn, $"%{request.Search}%") ||
                    EF.Functions.Like(m.DescriptionAr ?? "", $"%{request.Search}%") ||
                    EF.Functions.Like(m.DescriptionEn ?? "", $"%{request.Search}%"))
                .OrderBy(m => m.DisplayOrder)
                .Take(10)
                .ToListAsync(cancellationToken);
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
