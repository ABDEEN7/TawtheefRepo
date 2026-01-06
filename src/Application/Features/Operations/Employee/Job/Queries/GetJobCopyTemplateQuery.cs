using Cortex.Mediator.Queries;
using FluentResults;
using Tawtheef.Application.Features.Operations.Employee.Job.DTOs;

namespace Tawtheef.Application.Features.Operations.Employee.Job.Queries;

public record GetJobCopyTemplateQuery(Guid JobId) : IQuery<IResult<JobCopyTemplateDto>>;
