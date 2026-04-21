using Application.Operation.Features.Employee.JobManagement.JobOperations.DTOs;
using MediatR;
using FluentResults;

namespace Application.Operation.Features.Employee.JobManagement.JobOperations.Commands;

public record UpdateJobResponsibilitiesCommand(Guid JobId, UpdateJobResponsibilitiesDto Data) : IRequest<IResult<Unit>>;
