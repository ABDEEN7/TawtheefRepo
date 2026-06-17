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
        var officeRepository = unitOfWork.GetEntityRepository<Office>();
        var searchTerm = request.Search?.Trim();

        var officeQuery = officeRepository.DbSet
            .AsNoTracking()
            .WhereIf(!string.IsNullOrWhiteSpace(searchTerm), o =>
                EF.Functions.Like(o.NameAr, $"%{searchTerm}%") ||
                EF.Functions.Like(o.NameEn, $"%{searchTerm}%") ||
                EF.Functions.Like(o.Code, $"%{searchTerm}%"));

        var totalCount = await officeQuery.CountAsync(cancellationToken);
        var pageOffices = await officeQuery
            .OrderBy(o => o.DisplayOrder)
            .ThenBy(o => o.NameEn)
            .ToPaginatedResultAsync(request, cancellationToken);

        var pageOfficeIds = pageOffices.Select(o => o.Id).ToList();
        if (pageOfficeIds.Count == 0)
        {
            return Result.Ok(new PaginatedResult<OfficeDto>([], totalCount, request.PageNumber, request.PageSize));
        }

        var officeDetails = await officeRepository.DbSet
            .AsNoTracking()
            .AsSplitQuery()
            .Include(o => o.OfficeAdmin)
            .Include(o => o.Country)
            .Include(o => o.SupportedCountries).ThenInclude(sc => sc.Country)
            .Where(o => pageOfficeIds.Contains(o.Id))
            .ToListAsync(cancellationToken);

        var officeDetailsById = officeDetails.ToDictionary(o => o.Id);
        var orderedOffices = pageOfficeIds
            .Where(officeDetailsById.ContainsKey)
            .Select(id => officeDetailsById[id])
            .ToList();

        var offices = new PaginatedResult<OfficeDto>(
            mapper.Map<List<OfficeDto>>(orderedOffices),
            totalCount,
            request.PageNumber,
            request.PageSize);

        return Result.Ok(offices);
    }
}

