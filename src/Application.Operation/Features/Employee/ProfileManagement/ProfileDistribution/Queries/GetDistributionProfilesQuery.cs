using Application.Operation.Features.Employee.ProfileManagement.ProfileDistribution.DTOs;
using MediatR;
using FluentResults;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Employee.ProfileManagement.ProfileDistribution.Queries;

public sealed record GetDistributionProfilesQuery(
    Guid? UserId,
    UserProfileStatus? Status = null,
    string? SearchTerm = null,
    Guid? TargetEntityId = null)
    : PaginatedRequest, IRequest<Result<PaginatedResult<DistributionProfileDto>>>;

