using FluentResults;
using MapsterMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Models;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Extensions;
using Tawtheef.Application.Features.Operations.Employee.ManagementMajorSkill.Majors.Queries;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Tawtheef.Application.Features.Operations.Employee.ManagementMajorSkill.Majors.Handlers.Queries;

public class GetMainMajorsQueryHandler(IUnitOfWork uow, IMapper mapper) : IRequestHandler<GetMainMajorsQuery, IResult<PaginatedResult<DropdownOptions>>>
{
    public async Task<IResult<PaginatedResult<DropdownOptions>>> Handle(GetMainMajorsQuery request, CancellationToken cancellationToken)
    {
        var majors = await uow.GetEntityRepository<Major>().DbSet.AsNoTracking()
            .Where(x => x.IsActive)
            .WhereIf(!string.IsNullOrEmpty(request.Search),
                s =>
                    EF.Functions.Like(s.NameAr, $"%{request.Search}%") ||
                    EF.Functions.Like(s.NameEn, $"%{request.Search}%") ||
                    EF.Functions.Like(s.DescriptionAr ?? "", $"%{request.Search}%") ||
                    EF.Functions.Like(s.DescriptionEn ?? "", $"%{request.Search}%"))
            .OrderBy(x => x.GetLocalizedName(request.Language))
            .ToPaginatedListAsync<Major, DropdownOptions>(mapper, request, cancellationToken);

        return Result.Ok(majors);
    }
}
