using Application.Operation.Features.Employee.JobCandidates.Models;

namespace Application.Operation.Features.Employee.JobCandidates.Services.Interfaces;

public interface IJobRequirementsService
{
    Task<JobRequirements> GetAsync(Guid mainMajorId, Guid? subMajorId);
}
