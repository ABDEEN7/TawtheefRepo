using FluentResults;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;

namespace Tawtheef.Application.Common.Interfaces.Repositories;

public interface IJobSkillRepository : IBaseRepository<JobSkill>
{
    Task<IResult<List<JobSkill>>> GetByJobIdAsync(Guid jobId);
    Task<IResult<List<JobSkill>>> GetByJobIdOrderedAsync(Guid jobId);
    Task<IResult<int>> DeleteByJobIdAsync(Guid jobId);
}
