using Application.Operation.Features.Employee.JobManagement.JobCandidates.Models;
using Application.Operation.Features.Employee.JobManagement.JobCandidates.Queries;
using Application.Operation.Features.Employee.JobManagement.JobCandidates.Services.Interfaces;
using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.JobManagement.JobCandidates.Handlers.Queries;

public class CheckCandidateEligibilityQueryHandler(
    ICandidateEligibilityEvaluationService candidateEligibilityEvaluationService)
    : IRequestHandler<CheckCandidateEligibilityQuery, IResult<CandidateEligibilityResultDto>>
{
    public async Task<IResult<CandidateEligibilityResultDto>> Handle(CheckCandidateEligibilityQuery request, CancellationToken cancellationToken)
    {
        var result = await candidateEligibilityEvaluationService.EvaluateAsync(request.JobId, request.CandidateId, request.Language, cancellationToken);
        return result;
    }
}
