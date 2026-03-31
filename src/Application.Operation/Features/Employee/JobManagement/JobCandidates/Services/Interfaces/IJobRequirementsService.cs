using Application.Operation.Features.Employee.JobManagement.JobCandidates.Models;

namespace Application.Operation.Features.Employee.JobManagement.JobCandidates.Services.Interfaces;

public interface IJobRequirementsService
{
    Task<JobRequirements> GetAsync(Tawtheef.Domain.Entities.Recruitment.Job job);
}
