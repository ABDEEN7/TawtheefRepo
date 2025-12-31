using FluentResults;
using MapsterMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Extensions;
using Tawtheef.Application.Features.Operations.Admin.Universities.DTOs;
using Tawtheef.Application.Features.Operations.Admin.Universities.Queries;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Tawtheef.Application.Features.Operations.Admin.Universities.Handlers.Queries;

public sealed class ListUniversitiesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    : IRequestHandler<GetListUniversitiesQuery, IResult<PaginatedResult<UniversityAdminDto>>>
{
    public async Task<IResult<PaginatedResult<UniversityAdminDto>>> Handle(
        GetListUniversitiesQuery request,
        CancellationToken cancellationToken)
    {
        var searchTerm = request.Search?.Trim();

        var universities = await unitOfWork
            .GetEntityRepository<University>()
            .DbSet
            .AsNoTracking()
            .Include(u => u.City)!.ThenInclude(c => c.Country)
            .WhereIf(
                !string.IsNullOrWhiteSpace(searchTerm),
                u => EF.Functions.Like(u.NameEn, $"%{searchTerm}%") ||
                     EF.Functions.Like(u.NameAr, $"%{searchTerm}%"))
            .WhereIf(
                request.CountryId.HasValue,
                u => u.City != null && u.City.CountryId == request.CountryId)
            .OrderBy(u => u.DisplayOrder)
            .ThenBy(u => u.NameEn)
            .ToPaginatedListAsync<University, UniversityAdminDto>(mapper, request, cancellationToken);

        return Result.Ok(universities);
    }
}
