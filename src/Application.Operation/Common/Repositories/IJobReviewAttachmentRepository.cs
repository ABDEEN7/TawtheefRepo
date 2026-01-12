using FluentResults;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;

namespace Application.Operation.Common.Repositories;

public interface IJobReviewAttachmentRepository : IBaseRepository<JobReviewAttachment>
{
    Task<IResult<List<JobReviewAttachment>>> GetByIdWithDetailsAsync(Guid jobId);
    Task<IResult<List<JobReviewAttachment>>> GetLastReviewCycleAsync(Guid jobId);
}
