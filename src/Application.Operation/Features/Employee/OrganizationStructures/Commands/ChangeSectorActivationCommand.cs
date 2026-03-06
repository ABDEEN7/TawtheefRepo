using MediatR;
using FluentResults;

namespace Application.Operation.Features.Employee.OrganizationStructures.Commands;

public sealed record ChangeSectorActivationCommand(Guid Id, bool IsActive, bool ApplyOnHierarchy = true)
    : IRequest<IResult<Unit>>;

