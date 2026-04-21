using MediatR;
using FluentResults;

namespace Application.Operation.Features.Employee.JobManagement.JobOperations.Commands;

public sealed record RejectJobPointsCommand(
    Guid JobId,
    string Reason
) : IRequest<IResult<bool>>;
