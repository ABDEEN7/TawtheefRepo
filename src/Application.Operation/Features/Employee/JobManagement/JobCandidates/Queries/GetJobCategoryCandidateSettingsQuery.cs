using Application.Operation.Features.Employee.JobManagement.JobCandidates.DTOs;
using Cortex.Mediator.Queries;
using FluentResults;

namespace Application.Operation.Features.Employee.JobManagement.JobCandidates.Queries;

public sealed record GetJobCategoryCandidateSettingsQuery()
    : IQuery<IResult<JobCategoryCandidateSettingsResponseDto>>;
