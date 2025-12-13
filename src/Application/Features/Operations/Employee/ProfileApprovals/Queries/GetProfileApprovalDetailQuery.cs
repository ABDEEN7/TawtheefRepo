using FluentResults;
using MediatR;
using Tawtheef.Application.Features.Operations.Employee.ProfileApprovals.DTOs;

namespace Tawtheef.Application.Features.Operations.Employee.ProfileApprovals.Queries;

public record GetProfileApprovalDetailQuery(
    Guid UserProfileId,
    Guid OfficerId) : IRequest<Result<ProfileApprovalDetailDto>>;
