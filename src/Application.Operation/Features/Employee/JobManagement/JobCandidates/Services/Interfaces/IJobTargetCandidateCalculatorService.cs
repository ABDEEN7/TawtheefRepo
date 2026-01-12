namespace Application.Operation.Features.Employee.JobCandidates.Services.Interfaces;

public interface IJobTargetCandidateCalculatorService
{
    Task<int> GetTargetCountAsync(Guid jobCategoryId, int numberOfVacancies);
}
