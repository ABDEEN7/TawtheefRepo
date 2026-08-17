using Application.Operation.Features.Employee.JobManagement.JobCandidates.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;

namespace Application.Operation.Features.Employee.JobManagement.JobCandidates.Services;

public class JobTargetCandidateCalculatorService(IUnitOfWork unitOfWork) : IJobTargetCandidateCalculatorService
{
    public async Task<int> GetTargetCountAsync(Guid jobCategoryId, int numberOfVacancies)
    {
        var settings = await GetSettingsAsync(CancellationToken.None);

        return CalculateTargetCount(jobCategoryId, numberOfVacancies, settings);
    }

    public async Task<IReadOnlyDictionary<Guid, int>> GetTargetCountsAsync(
        IReadOnlyCollection<Job> jobs,
        CancellationToken cancellationToken)
    {
        if (jobs.Count == 0)
            return new Dictionary<Guid, int>();

        var settings = await GetSettingsAsync(cancellationToken);

        return jobs.ToDictionary(
            job => job.Id,
            job => CalculateTargetCount(job.JobCategoryId, job.NumberOfVacancies, settings));
    }

    private async Task<JobCategoryCandidateSettings> GetSettingsAsync(
        CancellationToken cancellationToken)
    {
        var settings = await unitOfWork
            .GetEntityRepository<JobCategoryCandidateSettings>()
            .DbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);

        if (settings is null)
            throw new InvalidOperationException("JobCategoryCandidateSettings not found.");

        return settings;
    }

    private static int CalculateTargetCount(
        Guid jobCategoryId,
        int numberOfVacancies,
        JobCategoryCandidateSettings settings)
    {
        return jobCategoryId switch
        {
            var id when id == JobCategoryIds.Academic =>
                numberOfVacancies * settings.AcademicJobVacancies,

            var id when id == JobCategoryIds.Administrative =>
                numberOfVacancies * settings.AdministrativeJobVacancies,

            var id when id == JobCategoryIds.Labor =>
                numberOfVacancies * settings.LaborJobVacancies,

            _ => numberOfVacancies
        };
    }
}
