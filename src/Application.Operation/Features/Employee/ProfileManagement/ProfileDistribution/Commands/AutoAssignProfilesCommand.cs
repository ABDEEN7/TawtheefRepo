using Application.Operation.Features.Employee.ProfileManagement.ProfileDistribution.DTOs;
using MediatR;
using FluentResults;

namespace Application.Operation.Features.Employee.ProfileManagement.ProfileDistribution.Commands;

public sealed record AutoAssignProfilesCommand(
    Guid UserId,
    IReadOnlyCollection<Guid> EmployeeIds,
    IReadOnlyCollection<Guid>? ProfileIds,
    int? PerEmployeeCount) : IRequest<Result<DistributionResultDto>>;

