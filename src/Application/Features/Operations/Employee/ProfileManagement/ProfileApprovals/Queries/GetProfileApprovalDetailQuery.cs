using Cortex.Mediator.Queries;
using FluentResults;

using Tawtheef.Application.Features.Operations.Employee.ProfileManagement.ProfileApprovals.DTOs;

namespace Tawtheef.Application.Features.Operations.Employee.ProfileManagement.ProfileApprovals.Queries;

public record GetProfileApprovalDetailQuery(
    Guid UserProfileId,
    Guid OfficerId) : IQuery<Result<GetProfileApprovalDetailDto>>;
