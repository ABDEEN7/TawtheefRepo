using Application.Operation.Features.Employee.ManagementMajorSkill.Skills.DTOs;
using Application.Operation.Features.Employee.ManagementMajorSkill.Skills.Queries;
using Cortex.Mediator.Queries;
using FluentResults;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Extensions;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Application.Operation.Features.Employee.ManagementMajorSkill.Skills.Handlers.Queries;


public sealed class GetSkillsQueryHandler(IUnitOfWork uow, IMapper mapper)
    : IQueryHandler<GetSkillsQuery, IResult<PaginatedResult<SkillDetailsDto>>>
{
    public async Task<IResult<PaginatedResult<SkillDetailsDto>>> Handle(GetSkillsQuery request, CancellationToken ct)
    {
        var skillUsageCounts = await uow.GetEntityRepository<MajorSkill>().DbSet.AsNoTracking()
            .GroupBy(x => x.SkillId)
            .Select(g => new { SkillId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.SkillId, x => x.Count, ct);

        var result = await uow.GetEntityRepository<Skill>().DbSet
            .AsNoTracking()
            .Include(b=> b.SkillType)
            .WhereIf(!string.IsNullOrWhiteSpace(request.Search), x =>
                x.NameAr.Contains(request.Search!.Trim()) ||
                x.NameEn.Contains(request.Search!.Trim()) ||
                x.BackendName.Contains(request.Search!.Trim()))
            .WhereIf(request.SkillTypeId.HasValue, x => x.SkillTypeId == request.SkillTypeId!.Value)
            .ToPaginatedListAsync<Skill, SkillDetailsDto>(mapper, request, ct);

        foreach (var skill in result.Items)
        {
            skill.UsedInMappingsCount = skillUsageCounts.TryGetValue(skill.Id, out var count) ? count : 0;
        }
        return Result.Ok(result);
    }
}
