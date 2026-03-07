using MediatR;
using FluentResults;

namespace Application.Operation.Features.Employee.ManagementMajorSkill.Majors.Commands;

public record ChangeActiveStatusMajorCommand(Guid Id, bool IsActive, bool ApplyOnRelationship = true) : IRequest<IResult<Unit>>;

