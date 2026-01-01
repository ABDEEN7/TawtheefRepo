using FluentResults;
using MapsterMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Extensions;
using Tawtheef.Application.Features.Operations.Employee.ManagementMajorSkill.Mapping.DTOs;
using Tawtheef.Application.Features.Operations.Employee.ManagementMajorSkill.Mapping.Queries;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Tawtheef.Application.Features.Operations.Employee.ManagementMajorSkill.Mapping.Handlers.Queries;

public sealed class GetMajorSkillsQueryHandler(IUnitOfWork uow, IMapper mapper)
    : IRequestHandler<GetMajorSkillsQuery, IResult<PaginatedResult<MajorSkillListItemDto>>>
{
    public async Task<IResult<PaginatedResult<MajorSkillListItemDto>>> Handle(GetMajorSkillsQuery request, CancellationToken ct)
    {
        var repo = uow.GetEntityRepository<MajorSkill>();
        var links = await repo.DbSet.AsNoTracking()
            .Include(m=> m.Major!.Parent)
            .Include(m=> m.Skill)
            .WhereIf(!string.IsNullOrWhiteSpace(request.Search),
                m => EF.Functions.Like(m.Major!.NameAr, $"%{request.Search}%")
                  || EF.Functions.Like(m.Major!.NameEn, $"%{request.Search}%")
                  || EF.Functions.Like(m.Skill!.NameAr, $"%{request.Search}%")
                  || EF.Functions.Like(m.Skill!.NameEn, $"%{request.Search}%"))
            .WhereIf(request.ParentMajorId.HasValue, m => m.Major!.ParentId == request.ParentMajorId)
            .WhereIf(request.SubMajorId.HasValue, m => m.MajorId == request.SubMajorId)
            .WhereIf(request.SkillTypeId.HasValue, m => m.Skill!.SkillTypeId == request.SkillTypeId)
            .WhereIf(request.IsActive.HasValue, m => m.IsActive == request.IsActive)
            .ToPaginatedListAsync<MajorSkill, MajorSkillListItemDto>(mapper, request, ct);
        return Result.Ok(links);
    }
}
