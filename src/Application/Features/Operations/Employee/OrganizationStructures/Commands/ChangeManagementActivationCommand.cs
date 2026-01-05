using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;

namespace Tawtheef.Application.Features.Operations.Employee.OrganizationStructures.Commands;

public sealed record ChangeManagementActivationCommand(Guid Id, bool IsActive, bool ApplyOnHierarchy = true)
    : ICommand<IResult<Unit>>;
