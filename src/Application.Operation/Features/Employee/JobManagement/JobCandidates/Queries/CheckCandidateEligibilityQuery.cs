using Application.Operation.Features.Employee.JobManagement.JobCandidates.Models;
using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.JobManagement.JobCandidates.Queries;

public record CheckCandidateEligibilityQuery(Guid JobId, Guid CandidateId, string Language)
    : IRequest<IResult<CandidateEligibilityResultDto>>;
