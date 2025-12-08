using FluentResults;
using MediatR;

namespace Tawtheef.Application.Features.Operations.Employee.Job.Commands;

public record ChangeJobStatusCommand(Guid JobId,Guid NewStatusId) : IRequest<IResult<Unit>>;
