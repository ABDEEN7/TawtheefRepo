using FluentResults;
using MediatR;
using Tawtheef.Application.Features.Operations.ProfileDistribution.DTOs;

namespace Tawtheef.Application.Features.Operations.Employee.ProfileDistribution.Commands;

public sealed record ManualAssignProfilesCommand(Guid EmployeeId, IReadOnlyCollection<Guid> ProfileIds)
    : IRequest<Result<DistributionResultDto>>;
