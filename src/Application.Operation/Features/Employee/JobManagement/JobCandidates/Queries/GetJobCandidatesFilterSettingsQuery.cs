using Application.Operation.Features.Employee.JobManagement.JobCandidates.DTOs;
using MediatR;
using FluentResults;

namespace Application.Operation.Features.Employee.JobManagement.JobCandidates.Queries;

public sealed record GetJobCandidatesFilterSettingsQuery(Guid JobId)
    : IRequest<IResult<JobCandidateFilterSettingsDto>>;

