using Application.Operation.Features.Employee.JobManagement.Job.DTOs;
using Cortex.Mediator.Queries;
using FluentResults;

namespace Application.Operation.Features.Employee.JobManagement.Job.Queries;
public record GetLatestJobTabReviewsQuery(Guid JobId) : IQuery<IResult<List<JobTabReviewNoteResponseDto>>>;
