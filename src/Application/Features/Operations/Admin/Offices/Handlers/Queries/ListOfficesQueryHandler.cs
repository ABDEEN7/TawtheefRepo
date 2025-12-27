using FluentResults;
using MapsterMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Extensions;
using Tawtheef.Application.Features.Operations.Admin.Offices.DTOs;
using Tawtheef.Application.Features.Operations.Admin.Offices.Queries;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Tawtheef.Application.Features.Operations.Admin.Offices.Handlers.Queries;

public sealed class ListOfficesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    : IRequestHandler<GetListOfficesQuery, IResult<PaginatedResult<OfficeDto>>>
{
    public async Task<IResult<PaginatedResult<OfficeDto>>> Handle(
        GetListOfficesQuery request,
        CancellationToken cancellationToken)
    {
        var offices = await unitOfWork.GetEntityRepository<Office>().DbSet
            .AsNoTracking()
            .Include(a => a.OfficeAdmin)
            .Include(o => o.Country)
            .Include(o => o.SupportedCountries)!.ThenInclude(sc => sc.Country)
            .Include(o => o.OfficeUsers)
            .WhereIf(!string.IsNullOrWhiteSpace(request.Search), o => o.NameAr.Contains(request.Search!) || 
                                                                      o.NameEn.Contains(request.Search!) || 
                                                                      o.Code.Contains(request.Search!))
            .ToPaginatedListAsync<Office, OfficeDto>(mapper, request, cancellationToken);

        return Result.Ok(offices);
    }
}
