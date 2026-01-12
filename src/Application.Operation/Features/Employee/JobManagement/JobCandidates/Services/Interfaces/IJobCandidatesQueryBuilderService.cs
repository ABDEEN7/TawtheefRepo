using Application.Operation.Features.Employee.JobManagement.JobCandidates.Models;

namespace Application.Operation.Features.Employee.JobManagement.JobCandidates.Services.Interfaces;

public interface IJobCandidatesQueryBuilderService
{
    IQueryable<JobCandidateRecord> BuildEligibleQuery(Guid jobId, Guid? jobGenderId, int jobMaximumAge,
        int jobMinimumAge, JobRequirements req, JobCandidatesFilter? filter);
}
