using Application.Operation.Features.Admin.Offices.DTOs;
using Application.Operation.Features.Admin.Offices.Queries;
using MediatR;
using FluentResults;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Extensions;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Application.Operation.Features.Admin.Offices.Handlers.Queries;

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
            .Include(o => o.SupportedCountries).ThenInclude(sc => sc.Country)
            .Include(o => o.OfficeUsers)
            .WhereIf(!string.IsNullOrWhiteSpace(request.Search), o => o.NameAr.Contains(request.Search!) || 
                                                                      o.NameEn.Contains(request.Search!) || 
                                                                      o.Code.Contains(request.Search!))
            .ToPaginatedListAsync<Office, OfficeDto>(mapper, request, cancellationToken);

        return Result.Ok(offices);
    }
}

