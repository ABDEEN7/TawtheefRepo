using Application.Operation.Features.Employee.ProfileManagement.ProfileApprovals.DTOs.ProfileApproval;
using MediatR;
using FluentResults;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Domain.Entities.Recruitment;
using Application.Operation.Features.Employee.ProfileManagement.ProfileApprovals;

namespace Application.Operation.Features.Employee.ProfileManagement.ProfileApprovals.Queries;

public record GetProfileApprovalsQuery(
    Guid OfficerId,
    string? Search = null,
    IReadOnlyCollection<ReviewStatus>? Statuses = null,
    IReadOnlyCollection<Guid>? TargetEntityIds = null,
    IReadOnlyCollection<Guid>? CandidateTypeIds = null,
    IReadOnlyCollection<ProfileApprovalCandidateSource>? CandidateSources = null
) : PaginatedRequest, IRequest<Result<PaginatedResult<ProfileApprovalListItemDto>>>;

