using Application.Operation.Features.Employee.JobManagement.JobCandidates.DTOs;
using MediatR;
using FluentResults;

namespace Application.Operation.Features.Employee.JobManagement.JobCandidates.Commands;

public sealed record SaveJobCandidatesFilterSettingsCommand(JobCandidateFilterSettingsDto Request)
    : IRequest<IResult<JobCandidateFilterSettingsDto>>;

