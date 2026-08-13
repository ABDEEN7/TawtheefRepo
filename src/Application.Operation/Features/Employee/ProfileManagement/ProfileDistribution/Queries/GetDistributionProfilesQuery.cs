using Application.Operation.Features.Employee.ProfileManagement.ProfileDistribution.DTOs;
using MediatR;
using FluentResults;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Employee.ProfileManagement.ProfileDistribution.Queries;

public sealed record GetDistributionProfilesQuery(
    Guid? UserId,
    string? SearchTerm = null,
    IReadOnlyCollection<UserProfileStatus>? Statuses = null,
    Guid? AssignedEmployeeId = null,
    Guid? TargetEntityId = null,
    IReadOnlyCollection<Guid>? CandidateTypeIds = null,
    bool? HasOtherSpecialization = null,
    bool? HasOtherUniversity = null,
    bool IsQatarGraduate = false,
    IReadOnlyCollection<Guid>? DegreeIds = null)
    : PaginatedRequest, IRequest<Result<PaginatedResult<DistributionProfileDto>>>
{
    protected override int MaximumPageSize => 500;
}

