using Application.Operation.Features.Employee.JobManagement.JobCandidates.DTOs;
using MediatR;
using FluentResults;

namespace Application.Operation.Features.Employee.JobManagement.JobCandidates.Commands;

public sealed record SaveJobCategoryCandidateSettingsCommand(JobCategoryCandidateSettingsRequestDto Request)
    : IRequest<IResult<JobCategoryCandidateSettingsResponseDto>>;

