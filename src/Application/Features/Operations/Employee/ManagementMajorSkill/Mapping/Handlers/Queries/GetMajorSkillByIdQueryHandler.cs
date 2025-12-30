using FluentResults;
using MapsterMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Operations.Employee.ManagementMajorSkill.Mapping.DTOs;
using Tawtheef.Application.Features.Operations.Employee.ManagementMajorSkill.Mapping.Queries;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Tawtheef.Application.Features.Operations.Employee.ManagementMajorSkill.Mapping.Handlers.Queries;

public sealed class GetMajorSkillByIdQueryHandler(IUnitOfWork uow, IMapper mapper)
    : IRequestHandler<GetMajorSkillByIdQuery, IResult<MajorSkillDetailsDto>>
{
    public async Task<IResult<MajorSkillDetailsDto>> Handle(GetMajorSkillByIdQuery request, CancellationToken ct)
    {
        var repo = uow.GetEntityRepository<MajorSkill>();

        var link = await repo.DbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

        if (link is null)
            return Result.Fail<MajorSkillDetailsDto>(new Error(ErrorsCodes.MajorSkillLinkNotFound).WithMetadata("MajorSkillLinkId", request.Id));

        return Result.Ok(mapper.Map<MajorSkillDetailsDto>(link));
    }
}
