using Application.Operation.Common.Repositories;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;
using Tawtheef.Infrastructure.Repositories.Base;

namespace Tawtheef.Infrastructure.Repositories;

public class JobSpecializationRepository(IGenericRepository<JobSpecialization> repository) 
    : BaseRepository<JobSpecialization>(repository), IJobSpecializationRepository;
