using FluentResults;
using MediatR;
using Tawtheef.Application.Features.Operations.ProfileApprovals.DTOs;

namespace Tawtheef.Application.Features.Operations.ProfileApprovals.Queries;

public record GetProfileApprovalDetailQuery(Guid UserProfileId) : IRequest<Result<ProfileApprovalDetailDto>>;
