using Application.Operation.Features.Employee.JobManagement.JobOperations.DTOs;
using MediatR;
using FluentResults;

namespace Application.Operation.Features.Employee.JobManagement.JobOperations.Commands;

public record UpdateJobConditionsCommand(Guid JobId, UpdateJobConditionsDto Data) : IRequest<IResult<Unit>>;
