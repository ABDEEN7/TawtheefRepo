using FluentResults;
using MediatR;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Features.Operations.Employee.ProfileManagement.ProfileApprovals.DTOs;
using Tawtheef.Domain.Entities.Recruitment;

namespace Tawtheef.Application.Features.Operations.Employee.ProfileManagement.ProfileApprovals.Queries;

public record GetProfileApprovalsQuery(
    Guid OfficerId,
    string? Search = null,
    string? Specialization = null,
    ReviewStatus? Status = null,
    string? TargetEntity = null,
    string? CandidateType = null
) : PaginatedRequest, IRequest<Result<PaginatedResult<ProfileApprovalListItemDto>>>;
