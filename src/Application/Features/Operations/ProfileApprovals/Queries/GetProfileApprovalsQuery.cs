using FluentResults;
using MediatR;
using Tawtheef.Application.Features.Operations.ProfileApprovals.DTOs;

namespace Tawtheef.Application.Features.Operations.ProfileApprovals.Queries;

public record GetProfileApprovalsQuery : IRequest<Result<IReadOnlyList<ProfileApprovalListItemDto>>>;
