using FluentResults;
using MediatR;
using Tawtheef.Application.Features.Operations.Employee.Job.DTOs;

namespace Tawtheef.Application.Features.Operations.Employee.Job.Commands;

public sealed record SaveJobReviewCommand(
    Guid JobId,
    SaveJobReviewRequestDto Request
) : IRequest<IResult<Unit>>;
