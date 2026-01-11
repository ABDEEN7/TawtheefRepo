using System.ComponentModel.DataAnnotations;
using Cortex.Mediator.Commands;
using FluentResults;

using Tawtheef.Application.Features.Operations.Employee.ProfileManagement.ProfileDistribution.DTOs;

namespace Tawtheef.Application.Features.Operations.Employee.ProfileManagement.ProfileDistribution.Commands;

public sealed record ReassignProfilesCommand(
    Guid UserId,
    [AllowedValues("Auto", "Manual")]
    string Mode,
    Guid? EmployeeId,
    IReadOnlyCollection<Guid> EmployeeIds,
    IReadOnlyCollection<Guid> ProfileIds,
    int? PerEmployeeCount) : ICommand<Result<DistributionResultDto>>;
