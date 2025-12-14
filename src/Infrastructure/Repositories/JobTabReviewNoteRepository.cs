using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;
using Tawtheef.Infrastructure.Repositories.Base;

namespace Tawtheef.Infrastructure.Repositories;
public class JobTabReviewNoteRepository(IGenericRepository<JobTabReviewNote> repository)
    : BaseRepository<JobTabReviewNote>(repository), IJobTabReviewNoteRepository
{
}
