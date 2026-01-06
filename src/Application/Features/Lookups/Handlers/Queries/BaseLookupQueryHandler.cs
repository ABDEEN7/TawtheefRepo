using Cortex.Mediator.Queries;
using FluentResults;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Models;
using Tawtheef.Application.Extensions;
using Tawtheef.Application.Features.Lookups.Queries;
using Tawtheef.Domain.Common;

namespace Tawtheef.Application.Features.Lookups.Handlers.Queries;

public abstract class BaseLookupQueryHandler<TLookup, TRequest>(IUnitOfWork unitOfWork, IMapper mapper)
    : IQueryHandler<TRequest, IResult<List<DropdownOptions>>>
    where TLookup : LookupBase
    where TRequest : BaseSearchQuery, IQuery<IResult<List<DropdownOptions>>>
{
    public async Task<IResult<List<DropdownOptions>>> Handle(TRequest request, CancellationToken cancellationToken)
    {
        var dbSet = unitOfWork.GetEntityRepository<TLookup>().DbSet;

        var entities = await dbSet
            .AsNoTracking()
            .Where(x=>x.IsActive)
            .WhereIf(!string.IsNullOrEmpty(request.Search), 
                s => 
                    EF.Functions.Like(s.NameAr, $"%{request.Search}%") ||
                    EF.Functions.Like(s.NameEn, $"%{request.Search}%") ||
                    EF.Functions.Like(s.DescriptionAr ?? "", $"%{request.Search}%") ||
                    EF.Functions.Like(s.DescriptionEn ?? "", $"%{request.Search}%"))
            .ToListAsync(cancellationToken);

        var data = mapper.Map<List<DropdownOptions>>(entities).OrderBy(e => e.Name).ToList();
        return Result.Ok(data);
    }
}
