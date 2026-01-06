using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;

namespace Tawtheef.Application.Features.Operations.Employee.ManagementMajorSkill.Majors.Commands;

public record ChangeActiveStatusMajorCommand(Guid Id, bool IsActive, bool ApplyOnRelationship = true) : ICommand<IResult<Unit>>;
