using Tawtheef.Application.Features.Operations.Employee.JobCandidates.Models;

namespace Tawtheef.Application.Features.Operations.Employee.JobCandidates.Services.Interfaces;

public interface IJobCandidatesQueryBuilderService
{
    IQueryable<JobCandidateRecord> BuildEligibleQuery(Guid jobId, Guid? jobGenderId, int jobMaximumAge,
        int jobMinimumAge, JobRequirements req, JobCandidatesFilter? filter);
}
