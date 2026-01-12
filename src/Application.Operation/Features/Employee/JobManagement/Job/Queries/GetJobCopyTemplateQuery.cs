using Application.Operation.Features.Employee.JobManagement.Job.DTOs;
using Cortex.Mediator.Queries;
using FluentResults;

namespace Application.Operation.Features.Employee.JobManagement.Job.Queries;

public record GetJobCopyTemplateQuery(Guid JobId) : IQuery<IResult<JobCopyTemplateDto>>;
