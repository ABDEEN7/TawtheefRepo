using FluentResults;
using MediatR;
using Tawtheef.Application.Features.Operations.Employee.ProfileManagement.ProfileDistribution.DTOs;

namespace Tawtheef.Application.Features.Operations.Employee.ProfileManagement.ProfileDistribution.Commands;

public sealed record ManualAssignProfilesCommand(Guid EmployeeId, IReadOnlyCollection<Guid> ProfileIds)
    : IRequest<Result<DistributionResultDto>>;
