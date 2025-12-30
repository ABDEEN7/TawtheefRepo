using FluentResults;
using MediatR;

namespace Tawtheef.Application.Features.Operations.Employee.ManagementMajorSkill.Majors.Commands;

public record ChangeActiveStatusMajorCommand(Guid Id, bool IsActive, bool ApplyOnRelationship = true) : IRequest<IResult<Unit>>;
