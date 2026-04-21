using MediatR;
using FluentResults;

namespace Application.Operation.Features.Employee.JobManagement.JobOperations.Commands;

public record DeleteJobCommand(Guid JobId) : IRequest<IResult<Unit>>;

