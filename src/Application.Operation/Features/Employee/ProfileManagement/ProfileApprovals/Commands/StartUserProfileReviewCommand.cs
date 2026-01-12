using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;

namespace Application.Operation.Features.Employee.ProfileManagement.ProfileApprovals.Commands;

public record StartUserProfileReviewCommand(
    Guid OfficerId,
    Guid UserProfileId
) : ICommand<IResult<Unit>>;
