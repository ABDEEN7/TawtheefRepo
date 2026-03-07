using MediatR;
using FluentResults;

namespace Application.Operation.Features.Employee.ManagementMajorSkill.Majors.Commands;

public record DeleteMajorCommand(Guid Id): IRequest<IResult<Unit>>;

