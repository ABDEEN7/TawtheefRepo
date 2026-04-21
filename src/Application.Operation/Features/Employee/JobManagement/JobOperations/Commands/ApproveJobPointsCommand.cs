using MediatR;
using FluentResults;

namespace Application.Operation.Features.Employee.JobManagement.JobOperations.Commands;

public record ApproveJobPointsCommand(Guid JobId) : IRequest<IResult<bool>>;

