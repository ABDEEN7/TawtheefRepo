using MediatR;
using FluentResults;

namespace Application.Operation.Features.Employee.ProfileManagement.ProfileApprovals.Commands;

public record StartUserProfileReviewCommand(
    Guid OfficerId,
    Guid UserProfileId
) : IRequest<IResult<Unit>>;

