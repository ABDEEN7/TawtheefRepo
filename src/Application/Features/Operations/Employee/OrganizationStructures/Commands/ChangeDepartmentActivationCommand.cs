using FluentResults;
using MediatR;

namespace Tawtheef.Application.Features.Operations.Employee.OrganizationStructures.Commands;

public sealed record ChangeDepartmentActivationCommand(Guid Id, bool IsActive)
    : IRequest<IResult<Unit>>;
