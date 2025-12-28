using FluentResults;
using MediatR;

namespace Tawtheef.Application.Features.Operations.Employee.Job.Commands;

public record ApproveJobPointsCommand(Guid JobId) : IRequest<IResult<bool>>;
