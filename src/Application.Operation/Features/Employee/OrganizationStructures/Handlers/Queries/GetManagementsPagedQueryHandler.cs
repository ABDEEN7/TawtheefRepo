using Application.Operation.Features.Employee.OrganizationStructures.DTOs;
using Application.Operation.Features.Employee.OrganizationStructures.Queries;
using MediatR;
using FluentResults;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Extensions;
using Tawtheef.Domain.Entities.Lookups;

namespace Application.Operation.Features.Employee.OrganizationStructures.Handlers.Queries;

public sealed class GetManagementsPagedQueryHandler(IUnitOfWork uow, IMapper mapper)
    : IRequestHandler<GetManagementsPagedQuery, IResult<PaginatedResult<ManagementDto>>>
{
    public async Task<IResult<PaginatedResult<ManagementDto>>> Handle(GetManagementsPagedQuery request, CancellationToken cancellationToken)
    {
        var query = uow.GetEntityRepository<Management>().DbSet
            .AsNoTracking()
            .Include(m => m.Sector)
            .WhereIf(request.SectorId.HasValue, m => m.SectorId == request.SectorId)
            .WhereIf(!string.IsNullOrWhiteSpace(request.Search),
                s =>
                    EF.Functions.Like(s.NameAr, $"%{request.Search}%") ||
                    EF.Functions.Like(s.NameEn, $"%{request.Search}%") ||
                    EF.Functions.Like(s.DescriptionAr ?? "", $"%{request.Search}%") ||
                    EF.Functions.Like(s.DescriptionEn ?? "", $"%{request.Search}%"))
            .WhereIf(request.IsActive.HasValue, s => s.IsActive == request.IsActive!.Value)
            .OrderBy(s => s.DisplayOrder);

        var result = await query.ToPaginatedListAsync<Management, ManagementDto>(mapper, request, cancellationToken);
        return Result.Ok(result);
    }
}

