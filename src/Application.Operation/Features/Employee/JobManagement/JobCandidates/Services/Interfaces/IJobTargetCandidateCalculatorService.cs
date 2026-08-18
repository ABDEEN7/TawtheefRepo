using Tawtheef.Domain.Entities.Recruitment;

namespace Application.Operation.Features.Employee.JobManagement.JobCandidates.Services.Interfaces;

public interface IJobTargetCandidateCalculatorService
{
    Task<int> GetTargetCountAsync(Guid jobCategoryId, int numberOfVacancies);

    Task<IReadOnlyDictionary<Guid, int>> GetTargetCountsAsync(
        IReadOnlyCollection<Job> jobs,
        CancellationToken cancellationToken);
}
