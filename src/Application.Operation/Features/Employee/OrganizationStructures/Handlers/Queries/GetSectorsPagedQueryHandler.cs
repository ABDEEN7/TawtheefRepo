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

public sealed class GetSectorsPagedQueryHandler(IUnitOfWork uow, IMapper mapper)
    : IRequestHandler<GetSectorsPagedQuery, IResult<PaginatedResult<SectorDto>>>
{
    public async Task<IResult<PaginatedResult<SectorDto>>> Handle(GetSectorsPagedQuery request, CancellationToken cancellationToken)
    {
        var query = uow.GetEntityRepository<Sector>().DbSet
            .AsNoTracking()
            .WhereIf(!string.IsNullOrWhiteSpace(request.Search),
                s =>
                    EF.Functions.Like(s.NameAr, $"%{request.Search}%") ||
                    EF.Functions.Like(s.NameEn, $"%{request.Search}%") ||
                    EF.Functions.Like(s.DescriptionAr ?? "", $"%{request.Search}%") ||
                    EF.Functions.Like(s.DescriptionEn ?? "", $"%{request.Search}%"))
            .WhereIf(request.IsActive.HasValue, s => s.IsActive == request.IsActive!.Value)
            .OrderBy(s => s.DisplayOrder);

        var result = await query.ToPaginatedListAsync<Sector, SectorDto>(mapper, request, cancellationToken);
        return Result.Ok(result);
    }
}

