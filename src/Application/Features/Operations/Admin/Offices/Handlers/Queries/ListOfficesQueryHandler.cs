using FluentResults;
using MapsterMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Models.Pagination;
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
        var officeRepo = unitOfWork.GetEntityRepository<Office>().DbSet
            .Include(o => o.Country)
            .Include(o => o.SupportedCountries)!.ThenInclude(sc => sc.Country)
            .Include(o => o.OfficeUsers);

        var query = officeRepo.AsQueryable();
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.Trim();
            query = query.Where(o => o.NameAr.Contains(term) || o.NameEn.Contains(term) || o.Code.Contains(term));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var pageNumber = request.PageNumber <= 0 ? 1 : request.PageNumber;
        var pageSize = request.PageSize <= 0 ? totalCount : request.PageSize;
        var skip = (pageNumber - 1) * pageSize;

        var offices = await query
            .OrderBy(o => o.DisplayOrder)
            .ThenBy(o => o.NameEn)
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = mapper.Map<List<OfficeDto>>(offices);

        return Result.Ok(new PaginatedResult<OfficeDto>(items, totalCount, pageNumber, pageSize));
    }
}
