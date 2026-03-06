using Application.Operation.Features.Employee.JobManagement.JobCandidates.DTOs;
using MediatR;
using FluentResults;

namespace Application.Operation.Features.Employee.JobManagement.JobCandidates.Queries;

public sealed record GetJobCandidateProfileQuery(Guid JobId, Guid CandidateId)
    : IRequest<IResult<JobCandidateProfileDto>>;

