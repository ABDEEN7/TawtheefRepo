using Application.Operation.Features.Employee.ManagementMajorSkill.Mapping.DTOs;
using MediatR;
using FluentResults;

namespace Application.Operation.Features.Employee.ManagementMajorSkill.Mapping.Queries;

public record GetMajorSkillByIdQuery(Guid Id)
    : IRequest<IResult<MajorSkillDetailsDto>>;

