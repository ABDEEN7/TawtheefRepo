using Application.Operation.Features.Employee.JobCandidates.DTOs;
using Cortex.Mediator.Commands;
using FluentResults;

namespace Application.Operation.Features.Employee.JobCandidates.Commands;

public sealed record SaveJobCandidatesFilterSettingsCommand(JobCandidateFilterSettingsDto Request)
    : ICommand<IResult<JobCandidateFilterSettingsDto>>;
