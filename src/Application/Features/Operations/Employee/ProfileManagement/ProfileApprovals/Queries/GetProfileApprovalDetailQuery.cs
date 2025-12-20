using FluentResults;
using MediatR;
using Tawtheef.Application.Features.Operations.Employee.ProfileManagement.ProfileApprovals.DTOs;

namespace Tawtheef.Application.Features.Operations.Employee.ProfileManagement.ProfileApprovals.Queries;

public record GetProfileApprovalDetailQuery(
    Guid UserProfileId,
    Guid OfficerId) : IRequest<Result<GetProfileApprovalDetailDto>>;
