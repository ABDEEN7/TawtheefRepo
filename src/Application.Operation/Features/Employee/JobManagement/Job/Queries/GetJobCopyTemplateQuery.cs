using Application.Operation.Features.Employee.Job.DTOs;
using Cortex.Mediator.Queries;
using FluentResults;

namespace Application.Operation.Features.Employee.Job.Queries;

public record GetJobCopyTemplateQuery(Guid JobId) : IQuery<IResult<JobCopyTemplateDto>>;
