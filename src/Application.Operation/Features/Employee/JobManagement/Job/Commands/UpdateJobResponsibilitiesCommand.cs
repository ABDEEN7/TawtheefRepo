using Application.Operation.Features.Employee.JobManagement.Job.DTOs;
using MediatR;
using FluentResults;

namespace Application.Operation.Features.Employee.JobManagement.Job.Commands;

public record UpdateJobResponsibilitiesCommand(Guid JobId, UpdateJobResponsibilitiesDto Data) : IRequest<IResult<Unit>>;
