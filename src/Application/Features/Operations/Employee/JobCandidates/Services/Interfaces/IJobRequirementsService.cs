using Tawtheef.Application.Features.Operations.Employee.JobCandidates.Models;

namespace Tawtheef.Application.Features.Operations.Employee.JobCandidates.Services.Interfaces;

public interface IJobRequirementsService
{
    Task<JobRequirements> GetAsync(Guid mainMajorId, Guid? subMajorId);
}
