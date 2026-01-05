using Cortex.Mediator.Queries;
using FluentResults;
using Tawtheef.Application.Features.Operations.Employee.ManagementMajorSkill.Mapping.DTOs;

namespace Tawtheef.Application.Features.Operations.Employee.ManagementMajorSkill.Mapping.Queries;

public record GetMajorSkillByIdQuery(Guid Id)
    : IQuery<IResult<MajorSkillDetailsDto>>;
