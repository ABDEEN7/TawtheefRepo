using Application.Operation.Features.Employee.JobManagement.JobCandidates.DTOs;
using Cortex.Mediator.Commands;
using FluentResults;

namespace Application.Operation.Features.Employee.JobManagement.JobCandidates.Commands;

public sealed record SaveJobCategoryCandidateSettingsCommand(JobCategoryCandidateSettingsRequestDto Request)
    : ICommand<IResult<JobCategoryCandidateSettingsResponseDto>>;
