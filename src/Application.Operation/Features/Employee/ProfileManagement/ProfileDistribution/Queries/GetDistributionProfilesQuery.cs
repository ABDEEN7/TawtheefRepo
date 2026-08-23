using Application.Operation.Features.Employee.ProfileManagement.ProfileDistribution.DTOs;
using MediatR;
using FluentResults;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Employee.ProfileManagement.ProfileDistribution.Queries;

public enum DistributionAssignmentState
{
    All = 0,
    Assigned = 1,
    Unassigned = 2
}

public sealed record GetDistributionProfilesQuery(
    Guid? UserId,
    string? SearchTerm = null,
    IReadOnlyCollection<UserProfileStatus>? Statuses = null,
    DistributionAssignmentState? AssignmentState = null,
    Guid? AssignedEmployeeId = null,
    Guid? TargetEntityId = null,
    IReadOnlyCollection<Guid>? CandidateTypeIds = null,
    bool? HasOtherSpecialization = null,
    bool? HasOtherUniversity = null,
    bool IsQatarGraduate = false,
    IReadOnlyCollection<Guid>? DegreeIds = null,
    int? Year = null)
    : PaginatedRequest, IRequest<Result<PaginatedResult<DistributionProfileDto>>>
{
    protected override int MaximumPageSize => 500;
}

