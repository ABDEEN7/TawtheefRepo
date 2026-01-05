using FluentResults;
using MediatR;

namespace Tawtheef.Application.Features.Operations.Employee.OrganizationStructures.Commands;

public sealed record ChangeSectorActivationCommand(Guid Id, bool IsActive, bool ApplyOnHierarchy = true)
    : IRequest<IResult<Unit>>;
