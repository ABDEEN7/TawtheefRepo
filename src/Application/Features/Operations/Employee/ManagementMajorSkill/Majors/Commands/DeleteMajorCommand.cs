using FluentResults;
using MediatR;

namespace Tawtheef.Application.Features.Operations.Employee.ManagementMajorSkill.Majors.Commands;

public record DeleteMajorCommand(Guid Id): IRequest<IResult<Unit>>;
