using Cortex.Mediator.Commands;
using FluentResults;
using Tawtheef.Application.Features.Operations.Employee.JobCandidates.DTOs;

namespace Tawtheef.Application.Features.Operations.Employee.JobCandidates.Commands;

public sealed record SaveJobCandidatesFilterSettingsCommand(JobCandidateFilterSettingsDto Request)
    : ICommand<IResult<JobCandidateFilterSettingsDto>>;
