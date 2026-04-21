using MediatR;
using FluentResults;

namespace Application.Operation.Features.Employee.JobManagement.JobOperations.Commands;

public record ChangeJobStatusCommand(Guid JobId,Guid NewStatusId) : IRequest<IResult<Unit>>;

