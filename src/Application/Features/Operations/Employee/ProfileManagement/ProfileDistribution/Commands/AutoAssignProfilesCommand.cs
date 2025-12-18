using FluentResults;
using MediatR;
using Tawtheef.Application.Features.Operations.Employee.ProfileManagement.ProfileDistribution.DTOs;

namespace Tawtheef.Application.Features.Operations.Employee.ProfileManagement.ProfileDistribution.Commands;

public sealed record AutoAssignProfilesCommand(
    IReadOnlyCollection<Guid> EmployeeIds,
    IReadOnlyCollection<Guid>? ProfileIds,
    int? PerEmployeeCount) : IRequest<Result<DistributionResultDto>>;
