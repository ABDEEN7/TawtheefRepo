using MediatR;
using FluentResults;

namespace Application.Operation.Features.Employee.JobManagement.JobOperations.Queries;

public record CheckJobInvitationsQuery(Guid JobId) : IRequest<IResult<bool>>;
