using System.ComponentModel.DataAnnotations;
using Cortex.Mediator.Commands;
using FluentResults;

using Tawtheef.Application.Features.Operations.Employee.ProfileManagement.ProfileDistribution.DTOs;
using Tawtheef.Application.Features.Operations.Employee.ProfileManagement.ProfileDistribution;

namespace Tawtheef.Application.Features.Operations.Employee.ProfileManagement.ProfileDistribution.Commands;

public sealed record ReassignProfilesCommand(
    Guid UserId,
    [AllowedValues(ProfileDistributionModes.Auto, ProfileDistributionModes.Manual)]
    string Mode,
    Guid? EmployeeId,
    IReadOnlyCollection<Guid> EmployeeIds,
    IReadOnlyCollection<Guid> ProfileIds,
    int? PerEmployeeCount) : ICommand<Result<DistributionResultDto>>;
