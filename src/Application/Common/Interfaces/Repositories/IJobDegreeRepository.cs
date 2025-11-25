using FluentResults;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;

namespace Tawtheef.Application.Common.Interfaces.Repositories;

public interface IJobDegreeRepository : IBaseRepository<JobDegree>
{
    Task<IResult<List<JobDegree>>> GetByJobIdAsync(Guid jobId);
    Task<IResult<bool>> ExistsForJobAndDegreeAsync(Guid jobId, Guid degreeId);
    Task<IResult<int>> DeleteByJobIdAsync(Guid jobId);
}
