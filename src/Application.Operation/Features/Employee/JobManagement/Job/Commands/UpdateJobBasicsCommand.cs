using Application.Operation.Features.Employee.JobManagement.Job.DTOs;
using MediatR;
using FluentResults;

namespace Application.Operation.Features.Employee.JobManagement.Job.Commands;

public record UpdateJobBasicsCommand(Guid JobId, UpdateJobBasicsDto Data) : IRequest<IResult<Unit>>;
