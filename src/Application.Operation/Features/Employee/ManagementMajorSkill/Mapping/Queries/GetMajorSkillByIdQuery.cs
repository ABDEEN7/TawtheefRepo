using Application.Operation.Features.Employee.ManagementMajorSkill.Mapping.DTOs;
using Cortex.Mediator.Queries;
using FluentResults;

namespace Application.Operation.Features.Employee.ManagementMajorSkill.Mapping.Queries;

public record GetMajorSkillByIdQuery(Guid Id)
    : IQuery<IResult<MajorSkillDetailsDto>>;
