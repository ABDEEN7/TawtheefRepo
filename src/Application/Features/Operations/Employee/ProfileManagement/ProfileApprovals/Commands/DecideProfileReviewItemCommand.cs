using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;
using Tawtheef.Domain.Entities.Recruitment;

namespace Tawtheef.Application.Features.Operations.Employee.ProfileManagement.ProfileApprovals.Commands;

public sealed record DecideProfileReviewItemCommand(
    Guid OfficerId,
    Guid ReviewItemId,
    ReviewStatus Status,
    string? Note
) : ICommand<IResult<Unit>>;

