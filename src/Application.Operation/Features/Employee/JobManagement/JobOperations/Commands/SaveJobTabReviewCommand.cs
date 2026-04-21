using Application.Operation.Features.Employee.JobManagement.JobOperations.DTOs;
using MediatR;
using FluentResults;

namespace Application.Operation.Features.Employee.JobManagement.JobOperations.Commands;

public sealed record SaveJobReviewCommand(
    Guid JobId,
    SaveJobReviewRequestDto Request
) : IRequest<IResult<Unit>>;

