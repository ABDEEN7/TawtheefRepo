using System.ComponentModel.DataAnnotations;
using Application.Operation.Features.Employee.ProfileManagement.ProfileDistribution.DTOs;
using Cortex.Mediator.Commands;
using FluentResults;

namespace Application.Operation.Features.Employee.ProfileManagement.ProfileDistribution.Commands;

public sealed record ReassignProfilesCommand(
    Guid UserId,
    [AllowedValues(ProfileDistributionModes.Auto, ProfileDistributionModes.Manual)]
    string Mode,
    Guid? EmployeeId,
    IReadOnlyCollection<Guid> EmployeeIds,
    IReadOnlyCollection<Guid> ProfileIds,
    int? PerEmployeeCount) : ICommand<Result<DistributionResultDto>>;
