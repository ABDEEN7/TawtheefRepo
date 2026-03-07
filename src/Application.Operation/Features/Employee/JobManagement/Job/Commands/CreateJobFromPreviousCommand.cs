using Application.Operation.Features.Employee.JobManagement.Job.DTOs;
using MediatR;
using FluentResults;

namespace Application.Operation.Features.Employee.JobManagement.Job.Commands;

public record CreateJobFromPreviousCommand(
    Guid SourceJobId,
    CreateJobFromPreviousDto Job) : IRequest<IResult<Guid>>;

