using FluentResults;
using MapsterMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Models;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Extensions;
using Tawtheef.Application.Features.Operations.Employee.ManagementMajorSkill.Skills.Queries;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Tawtheef.Application.Features.Operations.Employee.ManagementMajorSkill.Skills.Handlers.Queries;


public sealed class GetSkillsQueryHandler(IUnitOfWork uow, IMapper mapper)
    : IRequestHandler<GetSkillsQuery, IResult<PaginatedResult<DropdownOptions>>>
{
    public async Task<IResult<PaginatedResult<DropdownOptions>>> Handle(GetSkillsQuery request, CancellationToken ct)
    {
        var result = await uow.GetEntityRepository<Skill>().DbSet
            .AsNoTracking()
            .WhereIf(!string.IsNullOrWhiteSpace(request.Search), x =>
                x.NameAr.Contains(request.Search!.Trim()) ||
                x.NameEn.Contains(request.Search!.Trim()) ||
                x.BackendName.Contains(request.Search!.Trim()))
            .WhereIf(request.SkillTypeId.HasValue, x => x.SkillTypeId == request.SkillTypeId!.Value)
            .ToPaginatedListAsync<Skill, DropdownOptions>(mapper, request, ct);
        return Result.Ok(result);
    }
}
