using FluentResults;
using MediatR;
using Tawtheef.Application.Features.Operations.Employee.ManagementMajorSkill.Mapping.DTOs;

namespace Tawtheef.Application.Features.Operations.Employee.ManagementMajorSkill.Mapping.Queries;

public record GetMajorSkillByIdQuery(Guid Id)
    : IRequest<IResult<MajorSkillDetailsDto>>;
