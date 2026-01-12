using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;

namespace Application.Operation.Features.Employee.ManagementMajorSkill.Majors.Commands;

public record ChangeActiveStatusMajorCommand(Guid Id, bool IsActive, bool ApplyOnRelationship = true) : ICommand<IResult<Unit>>;
