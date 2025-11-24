using FluentResults;
using MediatR;
using Tawtheef.Application.Features.Operations.Employee.Job.DTOs;

namespace Tawtheef.Application.Features.Operations.Employee.Job.Commands;

public record UpdateJobCommand(Guid JobId, UpdateJobDto Job) : IRequest<IResult<Unit>>;
