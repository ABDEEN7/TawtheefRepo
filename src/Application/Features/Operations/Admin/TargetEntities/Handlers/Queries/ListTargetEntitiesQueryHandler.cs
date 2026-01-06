using Cortex.Mediator.Queries;
using FluentResults;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Extensions;
using Tawtheef.Application.Features.Operations.Admin.TargetEntities.DTOs;
using Tawtheef.Application.Features.Operations.Admin.TargetEntities.Queries;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Application.Features.Operations.Admin.TargetEntities.Handlers.Queries;

public sealed class ListTargetEntitiesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    : IQueryHandler<GetListTargetEntitiesQuery, IResult<PaginatedResult<TargetEntityAdminDto>>>
{
    public async Task<IResult<PaginatedResult<TargetEntityAdminDto>>> Handle(
        GetListTargetEntitiesQuery request,
        CancellationToken cancellationToken)
    {
        var searchTerm = request.Search?.Trim();

        var targetEntities = await unitOfWork
            .GetEntityRepository<TargetEntity>()
            .DbSet
            .AsNoTracking()
            .WhereIf(
                !string.IsNullOrWhiteSpace(searchTerm),
                t => EF.Functions.Like(t.NameEn, $"%{searchTerm}%") ||
                     EF.Functions.Like(t.NameAr, $"%{searchTerm}%") ||
                     EF.Functions.Like(t.BackendName, $"%{searchTerm}%"))
            .OrderBy(t => t.DisplayOrder)
            .ThenBy(t => t.NameEn)
            .ToPaginatedListAsync<TargetEntity, TargetEntityAdminDto>(mapper, request, cancellationToken);

        return Result.Ok(targetEntities);
    }
}
