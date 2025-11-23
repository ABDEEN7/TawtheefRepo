using FluentResults;
using MediatR;
using Tawtheef.Application.Features.Operations.Employee.Job.DTOs;

namespace Tawtheef.Application.Features.Operations.Employee.Job.Commands;

public record UpdateJobCommand : IRequest<IResult<Unit>>
{
    public Guid JobId { get; init; }
    public UpdateJobDto Job { get; init; } = default!;
}
