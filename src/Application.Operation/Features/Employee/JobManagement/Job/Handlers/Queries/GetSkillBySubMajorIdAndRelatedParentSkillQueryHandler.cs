using Application.Operation.Features.Employee.JobManagement.Job.Queries;
using MediatR;
using FluentResults;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Models;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Application.Operation.Features.Employee.JobManagement.Job.Handlers.Queries;

public class GetSkillBySubMajorIdAndRelatedParentSkillQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<GetSkillBySubMajorIdAndRelatedParentSkillQuery, IResult<List<DropdownOptions>>>
{
    public async Task<IResult<List<DropdownOptions>>> Handle(GetSkillBySubMajorIdAndRelatedParentSkillQuery request, CancellationToken cancellationToken)
    {
        IQueryable<Skill> skillsQuery;
        
        if (request.SubMajorIds is null || !request.SubMajorIds.Any())
        {
            skillsQuery = unitOfWork.GetEntityRepository<Skill>().DbSet
                .AsNoTracking()
                .Where(s => s.IsActive && s.IsGeneral);
        }
        else
        {
            var majorIds = await unitOfWork.GetEntityRepository<Major>().DbSet
                .AsNoTracking()
                .Where(m => request.SubMajorIds.Contains(m.Id))
                .Select(m => new { m.Id, m.ParentId })
                .ToListAsync(cancellationToken);

            var filterIds = majorIds
                .SelectMany(m => m.ParentId is null
                    ? new[] { m.Id }
                    : new[] { m.Id, m.ParentId.Value })
                .Distinct()
                .ToList();

            var majorSkillIds = await unitOfWork.GetEntityRepository<MajorSkill>().DbSet
                .AsNoTracking()
                .Where(ms => ms.IsActive && filterIds.Contains(ms.MajorId))
                .Select(ms => ms.SkillId)
                .ToListAsync(cancellationToken);

            skillsQuery = unitOfWork.GetEntityRepository<Skill>().DbSet
                .AsNoTracking()
                .Where(s => s.IsActive && (s.IsGeneral || majorSkillIds.Contains(s.Id)));
        }

        var skills = await skillsQuery
            .OrderBy(s => s.DisplayOrder)
            .ToListAsync(cancellationToken);

        return Result.Ok(mapper.Map<List<DropdownOptions>>(skills));
    }
}

