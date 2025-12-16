using FluentResults;
using MediatR;

namespace Tawtheef.Application.Features.Operations.Employee.Job.Commands;

public sealed record UpdateJobTabReviewStatusCommand(
    Guid JobId
) : IRequest<IResult<Unit>>;
