using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Extensions;
using Tawtheef.Application.Features.Operations.Admin.Countries.DTOs;
using Tawtheef.Application.Features.Operations.Admin.Countries.Queries;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Tawtheef.Application.Features.Operations.Admin.Countries.Handlers.Queries;

public sealed class ListCountriesQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetListCountriesQuery, IResult<PaginatedResult<CountryAdminDto>>>
{
    public async Task<IResult<PaginatedResult<CountryAdminDto>>> Handle(
        GetListCountriesQuery request,
        CancellationToken cancellationToken)
    {
        var nameFilter = request.Name?.Trim();
        var queryable = unitOfWork.GetEntityRepository<Country>().DbSet
            .AsNoTracking()
            .WhereIf(!string.IsNullOrWhiteSpace(nameFilter),
                c => EF.Functions.Like(c.NameEn, $"%{nameFilter}%") ||
                     EF.Functions.Like(c.NameAr, $"%{nameFilter}%") ||
                     EF.Functions.Like(c.BackendName, $"%{nameFilter}%"))
            .WhereIf(request.IsActive.HasValue, c => c.IsActive == request.IsActive!.Value);

        var totalCount = await queryable.CountAsync(cancellationToken);

        var countries = await queryable
            .OrderBy(c => c.DisplayOrder)
            .ThenBy(c => c.NameEn)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(c => new CountryAdminDto
            {
                Id = c.Id,
                NameAr = c.NameAr,
                NameEn = c.NameEn,
                Code = c.Code,
                ISOCode = c.ISOCode,
                CodeAlpha = c.CodeAlpha,
                IsActive = c.IsActive
            })
            .ToListAsync(cancellationToken);

        var result = new PaginatedResult<CountryAdminDto>(countries, totalCount, request.PageNumber, request.PageSize);

        return Result.Ok(result);
    }
}
