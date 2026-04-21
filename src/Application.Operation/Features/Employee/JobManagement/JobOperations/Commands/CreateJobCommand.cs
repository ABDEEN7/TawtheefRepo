using Application.Operation.Features.Employee.JobManagement.JobOperations.DTOs;
using MediatR;
using FluentResults;

namespace Application.Operation.Features.Employee.JobManagement.JobOperations.Commands;

public record CreateJobCommand(CreateJobDto Job) : IRequest<IResult<Guid>>;

