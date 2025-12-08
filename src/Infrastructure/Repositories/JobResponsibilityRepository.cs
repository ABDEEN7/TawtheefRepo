using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;
using Tawtheef.Infrastructure.Repositories.Base;

namespace Tawtheef.Infrastructure.Repositories;

public class JobResponsibilityRepository(IGenericRepository<JobResponsibility> repository)
    : BaseRepository<JobResponsibility>(repository), IJobResponsibilityRepository
{
}
