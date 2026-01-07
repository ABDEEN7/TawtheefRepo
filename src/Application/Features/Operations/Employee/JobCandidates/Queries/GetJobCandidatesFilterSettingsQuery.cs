using Cortex.Mediator.Queries;
using FluentResults;
using Tawtheef.Application.Features.Operations.Employee.JobCandidates.DTOs;

namespace Tawtheef.Application.Features.Operations.Employee.JobCandidates.Queries;

public sealed record GetJobCandidatesFilterSettingsQuery(Guid JobId)
    : IQuery<IResult<JobCandidateFilterSettingsDto>>;
