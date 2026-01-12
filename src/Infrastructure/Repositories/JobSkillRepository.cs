using Application.Operation.Common.Repositories;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;
using Tawtheef.Infrastructure.Repositories.Base;

namespace Tawtheef.Infrastructure.Repositories;

public class JobSkillRepository(IGenericRepository<JobSkill> repository)
    : BaseRepository<JobSkill>(repository), IJobSkillRepository
{
}
