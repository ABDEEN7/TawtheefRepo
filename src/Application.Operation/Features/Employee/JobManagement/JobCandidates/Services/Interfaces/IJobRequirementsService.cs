using Application.Operation.Features.Employee.JobManagement.JobCandidates.Models;

namespace Application.Operation.Features.Employee.JobManagement.JobCandidates.Services.Interfaces;

public interface IJobRequirementsService
{
    Task<JobRequirements> GetAsync(Guid mainMajorId, Guid? subMajorId);
}
