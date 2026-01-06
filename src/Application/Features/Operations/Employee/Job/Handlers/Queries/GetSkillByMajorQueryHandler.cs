using Cortex.Mediator.Queries;
using FluentResults;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Models;
using Tawtheef.Application.Features.Operations.Employee.Job.Queries;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Tawtheef.Application.Features.Operations.Employee.Job.Handlers.Queries;

public class GetSkillByMajorQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) : IQueryHandler<GetSkillByMajorQuery, IResult<List<DropdownOptions>>>
{
    public async Task<IResult<List<DropdownOptions>>> Handle(GetSkillByMajorQuery request, CancellationToken cancellationToken)
    {
        var dbSet = unitOfWork.GetEntityRepository<MajorSkill>().DbSet;

        var entities = await dbSet
            .AsNoTracking()
            .Where(ms => ms.IsActive)
            .Where(ms => ms.MajorId == request.MajorId || ms.Major!.ParentId == request.MajorId)
            .Select(ms=> ms.Skill!)
            .OrderBy(s => s.DisplayOrder)
            .ToListAsync(cancellationToken);

        return Result.Ok(mapper.Map<List<DropdownOptions>>(entities));
    }
}
