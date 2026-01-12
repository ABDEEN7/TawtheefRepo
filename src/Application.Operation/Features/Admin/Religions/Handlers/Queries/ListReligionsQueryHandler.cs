using Application.Operation.Features.Admin.Religions.DTOs;
using Application.Operation.Features.Admin.Religions.Queries;
using Cortex.Mediator.Queries;
using FluentResults;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Extensions;
using Tawtheef.Domain.Entities.Lookups;

namespace Application.Operation.Features.Admin.Religions.Handlers.Queries;

public sealed class ListReligionsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    : IQueryHandler<GetListReligionsQuery, IResult<PaginatedResult<ReligionAdminDto>>>
{
    public async Task<IResult<PaginatedResult<ReligionAdminDto>>> Handle(
        GetListReligionsQuery request,
        CancellationToken cancellationToken)
    {
        var searchTerm = request.Search?.Trim();

        var religions = await unitOfWork
            .GetEntityRepository<Religion>()
            .DbSet
            .AsNoTracking()
            .WhereIf(
                !string.IsNullOrWhiteSpace(searchTerm),
                l => EF.Functions.Like(l.NameEn, $"%{searchTerm}%") ||
                     EF.Functions.Like(l.NameAr, $"%{searchTerm}%"))
            .OrderBy(l => l.DisplayOrder)
            .ThenBy(l => l.NameEn)
            .ToPaginatedListAsync<Religion, ReligionAdminDto>(mapper, request, cancellationToken);

        return Result.Ok(religions);
    }
}
