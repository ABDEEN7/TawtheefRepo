using Application.Operation.Features.Employee.JobManagement.Job.DTOs;
using MediatR;
using FluentResults;

namespace Application.Operation.Features.Employee.JobManagement.Job.Queries;

public record GetJobCopyTemplateQuery(Guid JobId) : IRequest<IResult<JobCopyTemplateDto>>;

