using FluentResults;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;

namespace Tawtheef.Application.Common.Interfaces.Repositories;

public interface IJobPointsConfigurationsRepository : IBaseRepository<JobPointConfiguration>
{
    Task<IResult<JobPointConfiguration>> GetByJobIdAsync(Guid jobId);
}
