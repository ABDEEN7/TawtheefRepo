using MediatR;
using FluentResults;

namespace Application.Operation.Features.Employee.OrganizationStructures.Commands;

public sealed record ChangeDepartmentActivationCommand(Guid Id, bool IsActive)
    : IRequest<IResult<Unit>>;

