using FluentResults;
using MediatR;
using Tawtheef.Application.Features.Operations.ProfileDistribution.DTOs;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Operations.Employee.ProfileDistribution.Queries;

public sealed record GetDistributionProfilesQuery(UserProfileStatus? Status = null)
    : IRequest<Result<IReadOnlyList<DistributionProfileDto>>>;
