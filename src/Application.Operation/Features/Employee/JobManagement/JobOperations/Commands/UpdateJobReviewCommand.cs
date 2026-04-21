using Application.Operation.Features.Employee.JobManagement.JobOperations.DTOs;
using MediatR;
using FluentResults;

namespace Application.Operation.Features.Employee.JobManagement.JobOperations.Commands;

public sealed record UpdateJobReviewCommand(
    Guid JobId,
    SaveJobReviewRequestDto Request
) : IRequest<IResult<Unit>>;

