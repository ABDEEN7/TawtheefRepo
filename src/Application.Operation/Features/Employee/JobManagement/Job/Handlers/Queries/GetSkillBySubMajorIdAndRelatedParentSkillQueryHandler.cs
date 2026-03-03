using Application.Operation.Features.Employee.JobManagement.Job.Queries;
using Cortex.Mediator.Queries;
using FluentResults;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Models;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Application.Operation.Features.Employee.JobManagement.Job.Handlers.Queries;

public class GetSkillBySubMajorIdAndRelatedParentSkillQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) : IQueryHandler<GetSkillBySubMajorIdAndRelatedParentSkillQuery, IResult<List<DropdownOptions>>>
{
    public async Task<IResult<List<DropdownOptions>>> Handle(GetSkillBySubMajorIdAndRelatedParentSkillQuery request, CancellationToken cancellationToken)
    {
// 1) Load only ParentId (no Include)
        var majorIds = await unitOfWork.GetEntityRepository<Major>().DbSet
            .AsNoTracking()
            .Where(m => m.Id == request.SubMajorId)
            .Select(m => new { m.Id, m.ParentId })
            .FirstOrDefaultAsync(cancellationToken);

        if (majorIds is null)
            return Result.Fail<List<DropdownOptions>>(ErrorsCodes.MajorNotFound);

// 2) Build filter ids
        var ids = majorIds.ParentId is null
            ? new[] { majorIds.Id }
            : new[] { majorIds.Id, majorIds.ParentId.Value };

// 3) Query once with Contains
        var skills = await unitOfWork.GetEntityRepository<MajorSkill>().DbSet
            .AsNoTracking()
            .Where(ms => ms.IsActive && ids.Contains(ms.MajorId))        
            .Where(ms => ms.Skill != null)
            .Select(ms => ms.Skill!)              // ensure FK is enforced
            .Distinct()// optional, but recommended
            .OrderBy(s => s.DisplayOrder)
            .ToListAsync(cancellationToken);

        return Result.Ok(mapper.Map<List<DropdownOptions>>(skills));
    }
}
