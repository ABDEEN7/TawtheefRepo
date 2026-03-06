using MediatR;
using FluentResults;

namespace Application.Operation.Features.Employee.JobManagement.Job.Commands;

public record DeleteJobCommand(Guid JobId) : IRequest<IResult<Unit>>;

