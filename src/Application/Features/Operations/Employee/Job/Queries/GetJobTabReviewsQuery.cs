using Cortex.Mediator.Queries;
using FluentResults;
using Tawtheef.Application.Features.Operations.Employee.Job.DTOs;

namespace Tawtheef.Application.Features.Operations.Employee.Job.Queries;

public record GetJobTabReviewsQuery(Guid JobId) : IQuery<IResult<List<JobTabReviewNoteResponseDto>>>;
