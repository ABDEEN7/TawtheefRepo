using Application.Operation.Features.Admin.Cities.DTOs;
using Application.Operation.Features.Admin.Cities.Queries;
using MediatR;
using FluentResults;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Extensions;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Application.Operation.Features.Admin.Cities.Handlers.Queries;

public sealed class ListCitiesQueryHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper)
    : IRequestHandler<GetListCitiesQuery, IResult<PaginatedResult<CityAdminDto>>>
{
    public async Task<IResult<PaginatedResult<CityAdminDto>>> Handle(
        GetListCitiesQuery request,
        CancellationToken cancellationToken)
    {
        var nameFilter = request.Name?.Trim();

        var cities = await unitOfWork
            .GetEntityRepository<City>()
            .DbSet
            .AsNoTracking()
            .Include(c => c.Country)
            .WhereIf(
                request.CountryId.HasValue,
                c => c.CountryId == request.CountryId!.Value)
            .WhereIf(
                !string.IsNullOrWhiteSpace(nameFilter),
                c => EF.Functions.Like(c.NameEn, $"%{nameFilter}%") ||
                     EF.Functions.Like(c.NameAr, $"%{nameFilter}%") ||
                     EF.Functions.Like(c.BackendName, $"%{nameFilter}%"))
            .WhereIf(
                request.IsActive.HasValue,
                c => c.IsActive == request.IsActive!.Value)
            .OrderBy(c => c.DisplayOrder)
            .ThenBy(c => c.NameEn)
            .ToPaginatedListAsync<City, CityAdminDto>(
                mapper,
                request,
                cancellationToken);

        return Result.Ok(cities);
    }
}
