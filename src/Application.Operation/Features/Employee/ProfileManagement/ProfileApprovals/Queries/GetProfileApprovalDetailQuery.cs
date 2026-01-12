using Application.Operation.Features.Employee.ProfileManagement.ProfileApprovals.DTOs;
using Cortex.Mediator.Queries;
using FluentResults;

namespace Application.Operation.Features.Employee.ProfileManagement.ProfileApprovals.Queries;

public record GetProfileApprovalDetailQuery(
    Guid UserProfileId,
    Guid OfficerId) : IQuery<Result<GetProfileApprovalDetailDto>>;
