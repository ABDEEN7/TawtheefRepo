using FluentResults;
using MediatR;

namespace Tawtheef.Application.Features.Operations.Employee.Job.Commands;

public record DeleteJobCommand(Guid JobId) : IRequest<IResult<Unit>>;
