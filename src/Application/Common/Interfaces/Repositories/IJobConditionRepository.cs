using FluentResults;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;

namespace Tawtheef.Application.Common.Interfaces.Repositories;

public interface IJobConditionRepository : IBaseRepository<JobCondition>
{
    Task<IResult<List<JobCondition>>> GetByJobIdAsync(Guid jobId);
    Task<IResult<List<JobCondition>>> GetByJobIdOrderedAsync(Guid jobId);
    Task<IResult<int>> DeleteByJobIdAsync(Guid jobId);
}
