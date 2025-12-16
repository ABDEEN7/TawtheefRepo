using FluentResults;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;

namespace Tawtheef.Application.Common.Interfaces.Repositories;

public interface IJobTabReviewNoteRepository : IBaseRepository<JobTabReviewNote>
{
    Task<IResult<List<JobTabReviewNote>>> GetByIdWithDetailsAsync(Guid jobId);
    Task<IResult<List<JobTabReviewNote>>> GetLastReviewCycleAsync(Guid jobId);
}
