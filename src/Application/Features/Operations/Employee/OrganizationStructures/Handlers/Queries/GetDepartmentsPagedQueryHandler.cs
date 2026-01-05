using FluentResults;
using MapsterMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Extensions;
using Tawtheef.Application.Features.Operations.Employee.OrganizationStructures.DTOs;
using Tawtheef.Application.Features.Operations.Employee.OrganizationStructures.Queries;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Application.Features.Operations.Employee.OrganizationStructures.Handlers.Queries;

public sealed class GetDepartmentsPagedQueryHandler(IUnitOfWork uow, IMapper mapper)
    : IRequestHandler<GetDepartmentsPagedQuery, IResult<PaginatedResult<DepartmentDto>>>
{
    public async Task<IResult<PaginatedResult<DepartmentDto>>> Handle(GetDepartmentsPagedQuery request, CancellationToken cancellationToken)
    {
        var query = uow.GetEntityRepository<Department>().DbSet
            .AsNoTracking()
            .Include(d => d.Management)
            .ThenInclude(m => m!.Sector)
            .WhereIf(request.SectorId.HasValue, d => d.Management != null && d.Management.SectorId == request.SectorId)
            .WhereIf(request.ManagementId.HasValue, d => d.ManagementId == request.ManagementId)
            .WhereIf(!string.IsNullOrWhiteSpace(request.Search),
                s =>
                    EF.Functions.Like(s.NameAr, $"%{request.Search}%") ||
                    EF.Functions.Like(s.NameEn, $"%{request.Search}%") ||
                    EF.Functions.Like(s.DescriptionAr ?? "", $"%{request.Search}%") ||
                    EF.Functions.Like(s.DescriptionEn ?? "", $"%{request.Search}%"))
            .WhereIf(request.IsActive.HasValue, s => s.IsActive == request.IsActive!.Value)
            .OrderBy(s => s.DisplayOrder);

        var result = await query.ToPaginatedListAsync<Department, DepartmentDto>(mapper, request, cancellationToken);
        return Result.Ok(result);
    }
}
