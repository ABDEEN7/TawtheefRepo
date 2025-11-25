using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;
using Tawtheef.Infrastructure.Repositories.Base;

namespace Tawtheef.Infrastructure.Repositories;

public class JobQuotaRepository(IGenericRepository<JobQuota> repository)
    : BaseRepository<JobQuota>(repository), IJobQuotaRepository
{
    private readonly IGenericRepository<JobQuota> _repository = repository;
}
