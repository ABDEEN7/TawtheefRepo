using Application.Operation.Features.Employee.ManagementMajorSkill.Mapping.DTOs;
using Application.Operation.Features.Employee.ManagementMajorSkill.Mapping.Queries;
using Cortex.Mediator.Queries;
using FluentResults;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Application.Operation.Features.Employee.ManagementMajorSkill.Mapping.Handlers.Queries;

public sealed class GetMajorSkillByIdQueryHandler(IUnitOfWork uow, IMapper mapper)
    : IQueryHandler<GetMajorSkillByIdQuery, IResult<MajorSkillDetailsDto>>
{
    public async Task<IResult<MajorSkillDetailsDto>> Handle(GetMajorSkillByIdQuery request, CancellationToken ct)
    {
        var repo = uow.GetEntityRepository<MajorSkill>();

        var link = await repo.DbSet
            .AsNoTracking()
            .Include(ms=> ms.Major!.Parent)
            .Include(ms=> ms.Skill)
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

        if (link is null)
            return Result.Fail<MajorSkillDetailsDto>(new Error(ErrorsCodes.MajorSkillLinkNotFound).WithMetadata("MajorSkillLinkId", request.Id));

        return Result.Ok(mapper.Map<MajorSkillDetailsDto>(link));
    }
}
