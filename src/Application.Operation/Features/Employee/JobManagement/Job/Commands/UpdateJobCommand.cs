using Application.Operation.Features.Employee.JobManagement.Job.DTOs;
using MediatR;
using FluentResults;

namespace Application.Operation.Features.Employee.JobManagement.Job.Commands;

public record UpdateJobCommand(Guid JobId, UpdateJobDto Job) : IRequest<IResult<Unit>>;

