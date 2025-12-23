using FluentResults;
using MediatR;

namespace Tawtheef.Application.Features.Operations.Employee.ProfileManagement.ProfileApprovals.Commands;

public record StartUserProfileReviewCommand(
    Guid OfficerId,
    Guid UserProfileId
) : IRequest<IResult<Unit>>;
