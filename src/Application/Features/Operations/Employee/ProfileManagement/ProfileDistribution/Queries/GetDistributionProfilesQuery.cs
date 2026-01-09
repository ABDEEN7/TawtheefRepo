using Cortex.Mediator.Queries;
using FluentResults;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Features.Operations.Employee.ProfileManagement.ProfileDistribution.DTOs;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Operations.Employee.ProfileManagement.ProfileDistribution.Queries;

public sealed record GetDistributionProfilesQuery(Guid? UserId, UserProfileStatus? Status = null)
    : PaginatedRequest, IQuery<Result<PaginatedResult<DistributionProfileDto>>>;
