using MediatR;
using FluentResults;

namespace Application.Operation.Features.Employee.JobManagement.JobOperations.Commands;

public sealed record UpdateJobTabReviewStatusCommand(
    Guid JobId
) : IRequest<IResult<Unit>>;

