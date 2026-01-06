using Cortex.Mediator.Queries;
using FluentResults;

using Tawtheef.Application.Features.Operations.Employee.ProfileManagement.ProfileDistribution.DTOs;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Operations.Employee.ProfileManagement.ProfileDistribution.Queries;

public sealed record GetDistributionProfilesQuery(UserProfileStatus? Status = null)
    : IQuery<Result<IReadOnlyList<DistributionProfileDto>>>;
