using FluentResults;
using MediatR;
using Tawtheef.Application.Features.Operations.Employee.Job.Dtos;

namespace Tawtheef.Application.Features.Operations.Employee.Job.Queries;

public record GetJobByIdQuery(Guid jobId) : IRequest<IResult<JobResponseDto>>;

