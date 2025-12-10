using FluentResults;
using MediatR;
using Tawtheef.Application.Features.Operations.Employee.ProfileApprovals.DTOs;
using Tawtheef.Domain.Entities.Recruitment;

namespace Tawtheef.Application.Features.Operations.Employee.ProfileApprovals.Queries;

public record GetProfileApprovalsQuery(
    Guid OfficerId,
    string? Search = null,
    string? Specialization = null,
    ReviewStatus? Status = null,
    string? TargetEntity = null,
    string? CandidateType = null,
    string? Sort = null,
    string? SortDirection = null
) : IRequest<Result<IReadOnlyList<ProfileApprovalListItemDto>>>;
