using MediatR;
using FluentResults;

namespace Application.Operation.Features.Employee.JobManagement.Job.Commands;

public record ChangeJobStatusCommand(Guid JobId,Guid NewStatusId) : IRequest<IResult<Unit>>;

