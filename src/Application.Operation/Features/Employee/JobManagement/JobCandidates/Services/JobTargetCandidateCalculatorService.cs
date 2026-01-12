using Application.Operation.Features.Employee.JobManagement.JobCandidates.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;

namespace Application.Operation.Features.Employee.JobManagement.JobCandidates.Services;

public class JobTargetCandidateCalculatorService(IUnitOfWork unitOfWork) : IJobTargetCandidateCalculatorService
{
    public async Task<int> GetTargetCountAsync( Guid jobCategoryId,int numberOfVacancies)
    {
        var settings = await unitOfWork
            .GetEntityRepository<JobCategoryCandidateSettings>()
            .DbSet
            .AsNoTracking()
            .FirstOrDefaultAsync();

        if (settings is null)
            throw new InvalidOperationException("JobCategoryCandidateSettings not found.");

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
