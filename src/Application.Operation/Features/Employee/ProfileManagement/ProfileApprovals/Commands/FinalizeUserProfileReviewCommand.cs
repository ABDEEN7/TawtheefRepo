using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;

namespace Application.Operation.Features.Employee.ProfileManagement.ProfileApprovals.Commands;

public sealed record FinalizeUserProfileReviewCommand(
    Guid OfficerId,
    Guid UserProfileId,
    string? Notes,
    string? Summary
) : ICommand<IResult<Unit>>;
