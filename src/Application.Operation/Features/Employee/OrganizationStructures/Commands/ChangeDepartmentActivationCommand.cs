using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;

namespace Application.Operation.Features.Employee.OrganizationStructures.Commands;

public sealed record ChangeDepartmentActivationCommand(Guid Id, bool IsActive)
    : ICommand<IResult<Unit>>;
