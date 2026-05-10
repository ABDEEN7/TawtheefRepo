using Application.Operation.Features.Employee.JobManagement.JobCandidates.Models;
using FluentResults;

namespace Application.Operation.Features.Employee.JobManagement.JobCandidates.Services.Interfaces;


public interface ICandidateEligibilityEvaluationService
{
    Task<Result<CandidateEligibilityResultDto>> EvaluateAsync(Guid jobId, Guid candidateId, string language, 
        CancellationToken cancellationToken);
}
