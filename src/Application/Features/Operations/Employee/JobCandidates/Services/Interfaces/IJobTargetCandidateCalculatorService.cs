namespace Tawtheef.Application.Features.Operations.Employee.JobCandidates.Services.Interfaces;

public interface IJobTargetCandidateCalculatorService
{
    Task<int> GetTargetCountAsync(Guid jobCategoryId, int numberOfVacancies);
}
