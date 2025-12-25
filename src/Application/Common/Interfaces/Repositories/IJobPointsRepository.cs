using FluentResults;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;

namespace Tawtheef.Application.Common.Interfaces.Repositories;

public interface IJobPointsRepository : IBaseRepository<JobPointsMain>
{
    Task<IResult<JobPointsMain>> GetByJobIdAsync(Guid jobId);
}
