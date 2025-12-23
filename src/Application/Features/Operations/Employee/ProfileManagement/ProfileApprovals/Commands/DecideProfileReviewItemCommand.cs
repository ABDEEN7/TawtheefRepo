using FluentResults;
using MediatR;
using Tawtheef.Domain.Entities.Recruitment;

namespace Tawtheef.Application.Features.Operations.Employee.ProfileManagement.ProfileApprovals.Commands;

public sealed record DecideProfileReviewItemCommand(
    Guid OfficerId,
    Guid ReviewItemId,
    ReviewStatus Status,
    string? Note
) : IRequest<IResult<Unit>>;

