using MediatR;
using FluentResults;

namespace Application.Operation.Features.Employee.ProfileManagement.ProfileApprovals.Commands;

public sealed record FinalizeUserProfileReviewCommand(
    Guid OfficerId,
    Guid UserProfileId,
    string? Notes,
    string? Summary
) : IRequest<IResult<Unit>>;

