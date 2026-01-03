using FluentResults;
using MapsterMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Models;
using Tawtheef.Application.Extensions;
using Tawtheef.Application.Features.Lookups.Queries;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Tawtheef.Application.Features.Lookups.Handlers.Queries;

public sealed class GetOfficesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    : IRequestHandler<GetOfficesQuery, IResult<List<DropdownOptions>>>
{
    public async Task<IResult<List<DropdownOptions>>> Handle(GetOfficesQuery request, CancellationToken cancellationToken)
    {
        var offices = await unitOfWork.GetEntityRepository<Office>()
            .DbSet
            .Where(s => s.IsActive)
            .WhereIf(request.CountryId.HasValue, o => o.CountryId == request.CountryId!.Value)
            .WhereIf(!string.IsNullOrWhiteSpace(request.Search), o => 
                EF.Functions.Like(o.NameAr, $"%{request.Search}%") || 
                EF.Functions.Like(o.NameEn, $"%{request.Search}%"))
            .OrderBy(o => o.DisplayOrder)
            .ToListAsync(cancellationToken);

        return Result.Ok(mapper.Map<List<DropdownOptions>>(offices));
    }
}
