using Cortex.Mediator.Queries;
using FluentResults;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Models;
using Tawtheef.Application.Extensions;
using Tawtheef.Application.Features.Lookups.Queries;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Tawtheef.Application.Features.Lookups.Handlers.Queries;

public sealed class GetSkillsBasedOnMajorQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    : IQueryHandler<GetSkillsBasedOnMajorQuery, IResult<List<DropdownOptions>>>
{
    public async Task<IResult<List<DropdownOptions>>> Handle(GetSkillsBasedOnMajorQuery request,
        CancellationToken cancellationToken)
    {
        if (request.Majors.Count == 0) return Result.Ok(new List<DropdownOptions>());

        var entitiesQuery = unitOfWork.GetEntityRepository<MajorSkill>().DbSet
            .AsNoTracking()
            .Where(s => s.IsActive)
            .Where(s => request.Majors.Contains(s.MajorId))
            .Select(s => s.Skill!);

        List<Skill> entities;
        if (request.PaginatedRequest is not null)
            entities = await entitiesQuery.ToPaginatedResultAsync(request.PaginatedRequest, cancellationToken);
        else
            entities = await entitiesQuery.ToListAsync(cancellationToken);

        return Result.Ok(mapper.Map<List<DropdownOptions>>(entities));
    }
}
