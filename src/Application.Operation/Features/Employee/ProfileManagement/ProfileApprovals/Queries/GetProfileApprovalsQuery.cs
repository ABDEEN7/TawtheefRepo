using Application.Operation.Features.Employee.ProfileManagement.ProfileApprovals.DTOs.ProfileApproval;
using Cortex.Mediator.Queries;
using FluentResults;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Domain.Entities.Recruitment;

namespace Application.Operation.Features.Employee.ProfileManagement.ProfileApprovals.Queries;

public record GetProfileApprovalsQuery(
    Guid OfficerId,
    string? Search = null,
    string? Specialization = null,
    ReviewStatus? Status = null,
    Guid? TargetEntityId = null,
    string? CandidateType = null
) : PaginatedRequest, IQuery<Result<PaginatedResult<ProfileApprovalListItemDto>>>;
