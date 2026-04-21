using Application.Operation.Features.Employee.JobManagement.JobOperations.DTOs;
using MediatR;
using FluentResults;

namespace Application.Operation.Features.Employee.JobManagement.JobOperations.Commands;

public record UpdateJobBasicsCommand(Guid JobId, UpdateJobBasicsDto Data) : IRequest<IResult<Unit>>;
