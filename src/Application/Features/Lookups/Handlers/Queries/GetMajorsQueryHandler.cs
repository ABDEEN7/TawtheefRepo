using Cortex.Mediator.Queries;
using FluentResults;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Models;
using Tawtheef.Application.Extensions;
using Tawtheef.Application.Features.Lookups.Queries;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Tawtheef.Application.Features.Lookups.Handlers.Queries;

public sealed class GetMajorsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    : IQueryHandler<GetMajorsQuery, IResult<List<DropdownOptions>>>
{
    public async Task<IResult<List<DropdownOptions>>> Handle(GetMajorsQuery request, CancellationToken cancellationToken)
    {
        var dbSet = unitOfWork.GetEntityRepository<Major>().DbSet;

        var entities = await dbSet
            .AsNoTracking()
            .Where(s => s.IsActive)
            // we exclude majors that have no sub majors, because they are not selectable [Create Profile Page]
            .Where(m=> m.SubMajors!.Count > 0)
            .WhereIf(!string.IsNullOrEmpty(request.Search), 
                m => 
                    EF.Functions.Like(m.NameAr, $"%{request.Search}%") ||
                    EF.Functions.Like(m.NameEn, $"%{request.Search}%") ||
                    EF.Functions.Like(m.DescriptionAr ?? "", $"%{request.Search}%") ||
                    EF.Functions.Like(m.DescriptionEn ?? "", $"%{request.Search}%"))
            .OrderBy(m => m.DisplayOrder)
            .Take(10)
            .ToListAsync(cancellationToken);

        return Result.Ok(mapper.Map<List<DropdownOptions>>(entities));
    }
}
