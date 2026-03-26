using MediatR;
using FluentResults;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Models;
using Tawtheef.Application.Extensions;
using Tawtheef.Application.Features.Lookups.Queries;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Tawtheef.Application.Features.Lookups.Handlers.Queries;

public sealed class GetSkillsBasedOnMajorQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    : IRequestHandler<GetSkillsBasedOnMajorQuery, IResult<List<DropdownOptions>>>
{
    public async Task<IResult<List<DropdownOptions>>> Handle(GetSkillsBasedOnMajorQuery request,
        CancellationToken cancellationToken)
    {
        IQueryable<Skill> entitiesQuery;
        
        if (request.Majors.Count == 0)
        {
            entitiesQuery = unitOfWork.GetEntityRepository<Skill>().DbSet
                .AsNoTracking()
                .Where(s => s.IsActive && s.IsGeneral);
        }
        else
        {
            var majorSkillIds = await unitOfWork.GetEntityRepository<MajorSkill>().DbSet
                .AsNoTracking()
                .Where(s => s.IsActive && request.Majors.Contains(s.MajorId))
                .Select(s => s.SkillId)
                .ToListAsync(cancellationToken);

            entitiesQuery = unitOfWork.GetEntityRepository<Skill>().DbSet
                .AsNoTracking()
                .Where(s => s.IsActive && (s.IsGeneral || majorSkillIds.Contains(s.Id)));
        }

        List<Skill> entities;
        if (request.PaginatedRequest is not null)
            entities = await entitiesQuery.ToPaginatedResultAsync(request.PaginatedRequest, cancellationToken);
        else
            entities = await entitiesQuery.ToListAsync(cancellationToken);

        return Result.Ok(mapper.Map<List<DropdownOptions>>(entities));
    }
}

