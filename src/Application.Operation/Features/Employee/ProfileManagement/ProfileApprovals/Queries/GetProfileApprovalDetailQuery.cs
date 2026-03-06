using Application.Operation.Features.Employee.ProfileManagement.ProfileApprovals.DTOs;
using MediatR;
using FluentResults;

namespace Application.Operation.Features.Employee.ProfileManagement.ProfileApprovals.Queries;

public record GetProfileApprovalDetailQuery(
    Guid UserProfileId,
    Guid OfficerId) : IRequest<Result<GetProfileApprovalDetailDto>>;

