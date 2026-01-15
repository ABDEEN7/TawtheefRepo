using FluentResults;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;

namespace Application.Operation.Common.Repositories;

public interface IJobCategoryCandidateSettingsRepository : IBaseRepository<JobCategoryCandidateSettings>
{
    Task<IResult<JobCategoryCandidateSettings>> GetAsync();
}
