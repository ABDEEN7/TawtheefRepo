using FluentResults;
using MediatR;
using Tawtheef.Application.Features.Operations.ProfileDistribution.DTOs;

namespace Tawtheef.Application.Features.Operations.Employee.ProfileDistribution.Commands;

public sealed record ReassignProfilesCommand(
    string Mode,
    Guid? EmployeeId,
    IReadOnlyCollection<Guid> EmployeeIds,
    IReadOnlyCollection<Guid> ProfileIds,
    int? PerEmployeeCount) : IRequest<Result<DistributionResultDto>>;
