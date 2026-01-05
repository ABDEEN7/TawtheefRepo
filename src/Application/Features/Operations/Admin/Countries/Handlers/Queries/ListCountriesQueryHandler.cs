using Cortex.Mediator.Queries;
using FluentResults;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Extensions;
using Tawtheef.Application.Features.Operations.Admin.Countries.DTOs;
using Tawtheef.Application.Features.Operations.Admin.Countries.Queries;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Tawtheef.Application.Features.Operations.Admin.Countries.Handlers.Queries;

public sealed class ListCountriesQueryHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper)
    : IQueryHandler<GetListCountriesQuery, IResult<PaginatedResult<CountryAdminDto>>>
{
    public async Task<IResult<PaginatedResult<CountryAdminDto>>> Handle(
        GetListCountriesQuery request,
        CancellationToken cancellationToken)
    {
        var nameFilter = request.Name?.Trim();

        var countries = await unitOfWork
            .GetEntityRepository<Country>()
            .DbSet
            .AsNoTracking()
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
            .ToPaginatedListAsync<Country, CountryAdminDto>(
                mapper,
                request,
                cancellationToken);

        return Result.Ok(countries);
    }
}
