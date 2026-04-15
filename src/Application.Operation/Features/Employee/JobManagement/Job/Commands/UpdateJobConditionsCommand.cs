using Application.Operation.Features.Employee.JobManagement.Job.DTOs;
using MediatR;
using FluentResults;

namespace Application.Operation.Features.Employee.JobManagement.Job.Commands;

public record UpdateJobConditionsCommand(Guid JobId, UpdateJobConditionsDto Data) : IRequest<IResult<Unit>>;
